using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Features.Health;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Events;
using static api.mapserv.utah.gov.Features.Geocoding.DoubleAvenuesException;

namespace api.mapserv.utah.gov.Extensions {
    public static class ServiceCollectionExtensions {
        public static void UseOptions(this IServiceCollection services, IConfiguration config) {
            services.Configure<List<LocatorConfiguration>>(config.GetSection("webapi:locators"));
            services.Configure<GeometryServiceConfiguration>(config.GetSection("webapi:arcgis"));
            services.Configure<DatabaseConfiguration>(config.GetSection("webapi:database"));
        }

        public static void UseDi(this IServiceCollection services) {
            services.AddHttpClient("default", client => { client.Timeout = new TimeSpan(0, 0, 15); })
                    .ConfigurePrimaryHttpMessageHandler(() => {
                        var handler = new HttpClientHandler();
                        if (handler.SupportsAutomaticDecompression) {
                            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        }

                        return handler;
                    });

            services.AddSingleton<IAbbreviations, Abbreviations>();
            services.AddSingleton<IRegexCache, RegexCache>();
            services.AddSingleton<ILookupCache, LookupCache>();
            services.AddSingleton<IApiKeyRepository, PostgreApiKeyRepository>();
            services.AddSingleton<ICacheRepository, PostgreApiKeyRepository>();
            services.AddSingleton<IBrowserKeyProvider, AuthorizeApiKeyFromRequest.BrowserKeyProvider>();
            services.AddSingleton<IServerIpProvider, AuthorizeApiKeyFromRequest.ServerIpProvider>();
            services.AddTransient<IPipelineBehavior<ZoneParsing.Command, GeocodeAddress>, DoubleAvenueExceptionPipeline<ZoneParsing.Command, GeocodeAddress>>();
            services.AddSingleton<AuthorizeApiKeyFromRequest>();
            services.AddSingleton<ILogger>(provider => new LoggerConfiguration()
                                                       .MinimumLevel.Debug()
                                                       .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                                       .Enrich.FromLogContext()
                                                       .WriteTo.Console()
                                                       .CreateLogger());
            services.AddSingleton<IHealthCheck, CacheHealthCheck>();
            services.AddSingleton<IHealthCheck, KeyStoreHealthCheck>();
            services.AddSingleton<IHealthCheck, GeometryServiceHealthCheck>();
            services.AddSingleton<IHealthCheck, LocatorHealthCheck>();
        }
    }
}
