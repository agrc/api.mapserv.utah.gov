using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Features.GeometryService;
using AGRC.api.Features.Milepost;
using AGRC.api.Features.Searching;
using AGRC.api.Filters;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Configuration;
using AGRC.api.Services;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace AGRC.api.Extensions {
    public static class ServiceCollectionExtensions {
        public static void UseOptions(this IServiceCollection services, IConfiguration config) {
            services.Configure<List<LocatorConfiguration>>(config.GetSection("webapi:locators"));
            services.Configure<List<ReverseLocatorConfiguration>>(config.GetSection("webapi:locators"));
            services.Configure<GeometryServiceConfiguration>(config.GetSection("webapi:arcgis"));
            services.Configure<DatabaseConfiguration>(config.GetSection("webapi:database"));
        }

        public static void UseDi(this IServiceCollection services) {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                .WaitAndRetryAsync(3, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(8); // Timeout for an individual try

            services.AddHttpClient("default", client => client.Timeout = new TimeSpan(0, 0, 15))
                    .ConfigurePrimaryHttpMessageHandler(() => {
                        var handler = new HttpClientHandler();
                        if (handler.SupportsAutomaticDecompression) {
                            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        }

                        return handler;
                    })
                    .AddPolicyHandler(retryPolicy)
                    .AddPolicyHandler(timeoutPolicy);

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

            services.AddSingleton<IAbbreviations, Abbreviations>();
            services.AddSingleton<IRegexCache, RegexCache>();
            services.AddSingleton<ILookupCache, LookupCache>();
            services.AddSingleton<IApiKeyRepository, PostgreApiKeyRepository>();
            services.AddSingleton<ICacheRepository, PostgreApiKeyRepository>();
            services.AddSingleton<IBrowserKeyProvider, AuthorizeApiKeyFromRequest.BrowserKeyProvider>();
            services.AddSingleton<IServerIpProvider, AuthorizeApiKeyFromRequest.ServerIpProvider>();
            services.AddSingleton<AuthorizeApiKeyFromRequest>();
            services.AddSingleton<IDistanceStrategy, PythagoreanDistance>();
            services.AddSingleton<ITableMapping, TableMapping>();

            services.AddScoped<IFilterSuggestionFactory, FilterSuggestionFactory>();

            services.AddTransient<IPipelineBehavior<SearchQuery.Query, ObjectResult>,
                SearchQuery.ValidationBehavior<SearchQuery.Query, ObjectResult>>();

            services.AddTransient<IPipelineBehavior<GeocodeQuery.Query, ObjectResult>,
                GeocodeQuery.ValidationBehavior<GeocodeQuery.Query, ObjectResult>>();

            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogger<,>));
        }
    }
}
