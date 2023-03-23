using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Features.GeometryService;
using AGRC.api.Features.Health;
using AGRC.api.Features.Milepost;
using AGRC.api.Features.Searching;
using AGRC.api.Filters;
using AGRC.api.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Configuration;
using AGRC.api.Services;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using StackExchange.Redis;

#nullable enable
namespace AGRC.api.Extensions;
public static class ServiceCollectionExtensions {
    public static void UseOptions(this IServiceCollection services, IConfiguration config) {
        services.Configure<List<LocatorConfiguration>>(config.GetSection("webapi:locators"));
        services.Configure<List<ReverseLocatorConfiguration>>(config.GetSection("webapi:locators"));
        services.Configure<GeometryServiceConfiguration>(config.GetSection("webapi:geometryService"));
        services.Configure<DatabaseConfiguration>(config.GetSection("webapi:redis"));
    }

    public static void UseDi(this IServiceCollection services, IWebHostEnvironment env) {
        var retryPolicy = HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                        .WaitAndRetryAsync(3, retryAttempt =>
                                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(8);

        AddArcGisClient(services, retryPolicy, timeoutPolicy, env.IsProduction());

        services.AddHttpClient("health-check", client => client.Timeout = new TimeSpan(0, 0, 5))
                .ConfigurePrimaryHttpMessageHandler(() => {
                    var handler = new HttpClientHandler {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    if (handler.SupportsAutomaticDecompression) {
                        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    }

                    return handler;
                });

        services.AddHttpClient("udot", client => {
            client.BaseAddress = new Uri("https://maps.udot.utah.gov/");
            client.Timeout = new TimeSpan(0, 0, 15);
        }).ConfigurePrimaryHttpMessageHandler(() => {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression) {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            return handler;
        }).AddPolicyHandler(retryPolicy)
          .AddPolicyHandler(timeoutPolicy);

        services.AddHttpContextAccessor();

        var emulator = env.EnvironmentName switch {
            "Staging" or "Production" => EmulatorDetection.None,
            _ => EmulatorDetection.EmulatorOnly,
        };

        // This throws in dev but not prod if the database is not running
        services.AddSingleton(new FirestoreDbBuilder {
            ProjectId = Environment.GetEnvironmentVariable("GCLOUD_PROJECT") ?? "ut-dts-agrc-web-api-dev",
            EmulatorDetection = emulator
        }.Build());

        services.AddSingleton<IAbbreviations, Abbreviations>();
        services.AddSingleton<IRegexCache, RegexCache>();
        services.AddSingleton<IApiKeyRepository, FirestoreApiKeyRepository>();
        services.AddSingleton<ICacheRepository, RedisCacheRepository>();
        services.AddSingleton<IStaticCache, StaticCache>();
        services.AddSingleton<IBrowserKeyProvider, AuthorizeApiKeyFromRequest.BrowserKeyProvider>();
        services.AddSingleton<IServerIpProvider, AuthorizeApiKeyFromRequest.ServerIpProvider>();
        services.AddSingleton<AuthorizeApiKeyFromRequest>();
        services.AddSingleton<IDistanceStrategy, PythagoreanDistance>();
        services.AddSingleton<ITableMapping, TableMapping>();
        services.AddSingleton<StartupHealthCheck>();
        services.AddSingleton((provider) => {
            var options = provider.GetService<IOptions<DatabaseConfiguration>>();
            ArgumentNullException.ThrowIfNull(options);

            return new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options.Value.ConnectionString));
        });

        services.AddSingleton((provider) => {
            var options = provider.GetService<IOptions<SearchProviderConfiguration>>();
            ArgumentNullException.ThrowIfNull(options);

            var builder = new NpgsqlDataSourceBuilder(options.Value.ConnectionString);
            builder.UseLoggerFactory(provider.GetService<ILoggerFactory>());
            builder.UseNetTopologySuite();

            return builder.Build();
        });

        services.AddSingleton<IConfigureOptions<MvcOptions>>(sp => {
            var options = sp.GetRequiredService<IOptionsMonitor<JsonOptions>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new ConfigureMvcJsonOptions(options, loggerFactory);
        });

        services.Configure<HostOptions>(options =>
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);

        services.AddHostedService<StartupBackgroundService>();
        services.AddHostedService<CacheHostedService>();

        services.AddScoped<IFilterSuggestionFactory, FilterSuggestionFactory>();

        services.AddTransient<IPipelineBehavior<SearchQuery.Query, ObjectResult>,
            SearchQuery.ValidationBehavior<SearchQuery.Query, ObjectResult>>();

        services.AddTransient<IPipelineBehavior<GeocodeQuery.Query, ObjectResult>,
            GeocodeQuery.ValidationBehavior<GeocodeQuery.Query, ObjectResult>>();

        services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogger<,>));
    }

    private static void AddArcGisClient(IServiceCollection services, AsyncRetryPolicy<HttpResponseMessage> retryPolicy, AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy, bool isProduction) {
        var builder = services.AddHttpClient("arcgis", client => client.Timeout = new TimeSpan(0, 0, 15))
                .ConfigurePrimaryHttpMessageHandler(() => {
                    var handler = new HttpClientHandler {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    if (handler.SupportsAutomaticDecompression) {
                        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    }

                    return handler;
                });

        if (isProduction) {
            builder.AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(timeoutPolicy);
        }
    }
}
