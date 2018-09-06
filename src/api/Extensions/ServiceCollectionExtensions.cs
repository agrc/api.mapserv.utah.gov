using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Features.Health;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Configuration;
using api.mapserv.utah.gov.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using WebApiContrib.Core.Formatter.Jsonp;
using static api.mapserv.utah.gov.Features.Geocoding.DoubleAvenuesException;

namespace api.mapserv.utah.gov.Extensions {
    public static class ServiceCollectionExtensions {
        public static void AddEnvOptions(this IServiceCollection services, IConfiguration config) {
            services.Configure<List<LocatorConfiguration>>(config.GetSection("webapi:locators"));
            services.Configure<GeometryServiceConfiguration>(config.GetSection("webapi:arcgis"));
            services.Configure<DatabaseConfiguration>(config.GetSection("webapi:database"));
        }

        public static void AddDi(this IServiceCollection services) {
            services.AddMediatR(typeof(Startup));

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

            services.AddDistributedRedisCache(options => {
                options.Configuration = "localhost";
            });
        }

        public static void AddOpenApi(this IServiceCollection services) {
            services.AddSwaggerGen(c => {
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();

                c.SwaggerDoc("v1", new Info {
                    Version = "v1",
                    Title = "AGRC WebAPI : OpenAPI Documentation",
                    Description = "OpenAPI Documentation",
                    Contact = new Contact {
                        Name = "AGRC",
                        Email = string.Empty,
                        Url = "https://github.com/agrc/api.mapserv.utah.gov"
                    },
                    License = new License {
                        Name = "MIT",
                        Url = "https://github.com/agrc/api.mapserv.utah.gov/blob/master/LICENSE"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void AddMvcConfiguration(this IServiceCollection services) {
            services.AddCors();
            services.AddMvc(options => {
                options.AddResponseCache();
                options.AddApiResponseFormatters();
                options.AddJsonpOutputFormatter();
                options.AddResultCache();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => options.SerializerSettings.NullValueHandling =
                                       NullValueHandling.Ignore);

            services.AddApiVersioning(x => {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddHealthChecks();
        }
    }
}
