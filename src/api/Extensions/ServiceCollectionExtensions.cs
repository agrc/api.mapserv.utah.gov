using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Features.GeometryService;
using api.mapserv.utah.gov.Features.Health;
using api.mapserv.utah.gov.Features.Searching;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Infrastructure;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ApiResponses;
using api.mapserv.utah.gov.Models.ArcGis;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Services;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;
using Serilog.Events;
using static api.mapserv.utah.gov.Features.Geocoding.DoubleAvenuesException;

namespace api.mapserv.utah.gov.Extensions {
    public static class ServiceCollectionExtensions {
        public static void UseOptions(this IServiceCollection services, IConfiguration config) {
            services.Configure<List<LocatorConfiguration>>(config.GetSection("webapi:locators"));
            services.Configure<List<ReverseLocatorConfiguration>>(config.GetSection("webapi:locators"));
            services.Configure<GeometryServiceConfiguration>(config.GetSection("webapi:arcgis"));
            services.Configure<DatabaseConfiguration>(config.GetSection("webapi:database"));
        }

        public static void UseDi(this IServiceCollection services, IConfiguration config) {
            services.AddSingleton<ILogger>(provider => new LoggerConfiguration()
                                                       .ReadFrom.Configuration(config)
                                                       .CreateLogger());

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                .WaitAndRetryAsync(3, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(8); // Timeout for an individual try

            services.AddHttpClient("default", client => { client.Timeout = new TimeSpan(0, 0, 15); })
                    .ConfigurePrimaryHttpMessageHandler(() => {
                        var handler = new HttpClientHandler();
                        if (handler.SupportsAutomaticDecompression) {
                            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        }

                        return handler;
                    })
                    .AddPolicyHandler(retryPolicy)
                    .AddPolicyHandler(timeoutPolicy)
                    .SetHandlerLifetime(Timeout.InfiniteTimeSpan); // TODO: Remove after preview 3 or when fixed

            services.AddSingleton<IAbbreviations, Abbreviations>();
            services.AddSingleton<IRegexCache, RegexCache>();
            services.AddSingleton<ILookupCache, LookupCache>();
            services.AddSingleton<IApiKeyRepository, PostgreApiKeyRepository>();
            services.AddSingleton<ICacheRepository, PostgreApiKeyRepository>();
            services.AddSingleton<IBrowserKeyProvider, AuthorizeApiKeyFromRequest.BrowserKeyProvider>();
            services.AddSingleton<IServerIpProvider, AuthorizeApiKeyFromRequest.ServerIpProvider>();
            services.AddSingleton<AuthorizeApiKeyFromRequest>();

            services.AddTransient<IPipelineBehavior<SqlQuery.Command, IReadOnlyCollection<SearchApiResponse>>, KeyFormatting.Pipeline<SqlQuery.Command, IReadOnlyCollection<SearchApiResponse>>>();
            services.AddTransient<IPipelineBehavior<ZoneParsing.Command, AddressWithGrids>, DoubleAvenueExceptionPipeline<ZoneParsing.Command, AddressWithGrids>>();

            services.AddTransient<IPipelineBehavior<PoBoxLocation.Command, Candidate>, ReprojectPipeline<PoBoxLocation.Command, Candidate>>();
            services.AddTransient<IPipelineBehavior<UspsDeliveryPointLocation.Command, Candidate>, ReprojectPipeline<UspsDeliveryPointLocation.Command, Candidate>>();

            services.AddTransient<IRequestPreProcessor<SqlQuery.Command>, SqlPreProcessor>();
            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogger<,>));
        }
    }
}
