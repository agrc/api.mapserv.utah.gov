using System.IO;
using System.Net;
using System.Net.Http;
using ugrc.api.Cache;
using ugrc.api.Features.Converting;
using ugrc.api.Features.Geocoding;
using ugrc.api.Features.Health;
using ugrc.api.Features.Milepost;
using ugrc.api.Features.Searching;
using ugrc.api.Infrastructure;
using ugrc.api.Middleware;
using ugrc.api.Models.Configuration;
using ugrc.api.Models.ResponseContracts;
using ugrc.api.Services;
using api.OpenApi;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Google.Api.Gax;
using Google.Cloud.Firestore;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Npgsql;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ugrc.api.Extensions;
public static class WebApplicationBuilderExtensions {
    public static void ConfigureConfiguration(this WebApplicationBuilder builder) {
        builder.Configuration
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
         .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
         .AddEnvironmentVariables();

        if (!builder.Environment.IsDevelopment()) {
            builder.Configuration.AddJsonFile(
                Path.Combine(Path.DirectorySeparatorChar.ToString(), "secrets", "dotnet", "appsettings.Production.json"),
                optional: false,
                reloadOnChange: true
            );
        }
    }
    public static void ConfigureLogging(this WebApplicationBuilder builder) {
        builder.Logging.ClearProviders();
        builder.Host
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((context, provider) => {
                provider.ReadFrom.Configuration(context.Configuration);
            });
    }
    public static void ConfigureHealthChecks(this WebApplicationBuilder builder)
        => builder.Services.AddHealthChecks()
         .AddCheck<StartupHealthCheck>("Startup", failureStatus: HealthStatus.Degraded, tags: ["startup"])
         .AddCheck<CacheHealthCheck>("Cache", failureStatus: HealthStatus.Degraded, tags: ["health"])
         .AddCheck<KeyStoreHealthCheck>("KeyStore", failureStatus: HealthStatus.Unhealthy, tags: ["health"])
         .AddCheck<UdotServiceHealthCheck>("ArcGIS:RoadsAndHighwaysService", failureStatus: HealthStatus.Degraded, tags: ["health"])
         .AddCheck<LocatorHealthCheck>("ArcGIS:LocatorServices", tags: ["health"])
         .AddCheck<BigQueryHealthCheck>("Database", tags: ["health"]);
    public static void ConfigureDependencyInjection(this WebApplicationBuilder builder) {
        builder.Services.Configure<List<LocatorConfiguration>>(builder.Configuration.GetSection("webapi:locators"));
        builder.Services.Configure<List<ReverseLocatorConfiguration>>(builder.Configuration.GetSection("webapi:locators"));
        builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("webapi:redis"));

        builder.Services.AddHttpContextAccessor();

        var emulator = builder.Environment.EnvironmentName switch {
            "Staging" or "Production" => EmulatorDetection.None,
            _ => EmulatorDetection.EmulatorOnly,
        };

        // Singletons - same for every request
        // This throws in dev but not prod if the database is not running
        builder.Services.AddSingleton(new FirestoreDbBuilder {
            ProjectId = Environment.GetEnvironmentVariable("GCLOUD_PROJECT") ?? "ut-dts-agrc-web-api-dev",
            EmulatorDetection = emulator
        }.Build());
        builder.Services.AddSingleton<IJsonSerializerOptionsFactory, JsonSerializerOptionsFactory>();
        builder.Services.AddSingleton<IAbbreviations, Abbreviations>();
        builder.Services.AddSingleton<IRegexCache, RegexCache>();
        builder.Services.AddSingleton<IApiKeyRepository, FirestoreApiKeyRepository>();
        builder.Services.AddSingleton<ICacheRepository, RedisCacheRepository>();
        builder.Services.AddSingleton<IStaticCache, StaticCache>();
        builder.Services.AddSingleton<IBrowserKeyProvider, BrowserKeyProvider>();
        builder.Services.AddSingleton<IServerIpProvider, FirebaseClientIpProvider>();
        builder.Services.AddSingleton<IDistanceStrategy, PythagoreanDistance>();
        builder.Services.AddSingleton<ITableMapping, TableMapping>();
        builder.Services.AddSingleton<StartupHealthCheck>();
        builder.Services.AddSingleton((provider) => {
            var options = provider.GetService<IOptions<DatabaseConfiguration>>();
            ArgumentNullException.ThrowIfNull(options);

            return new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options.Value.ConnectionString));
        });
        builder.Services.AddSingleton((provider) => {
            var options = provider.GetService<IOptions<SearchProviderConfiguration>>();
            ArgumentNullException.ThrowIfNull(options);

            var builder = new NpgsqlDataSourceBuilder(options.Value.ConnectionString);
            builder.UseLoggerFactory(provider.GetService<ILoggerFactory>());
            builder.UseNetTopologySuite();

            return builder.Build();
        });

        builder.Services.Configure<HostOptions>(options =>
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore);

        builder.Services.AddHostedService<StartupBackgroundService>();
        builder.Services.AddHostedService<CacheHostedService>();

        // transient - always different for every injection
        builder.Services.AddTransient(typeof(IRequestPreProcessor<>), typeof(RequestLogger<>));
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogger<,>));

        // scoped - same within a request but different for each request
        builder.Services.AddScoped<IFilterSuggestionFactory, FilterSuggestionFactory>();

        builder.Host.ConfigureContainer<ContainerBuilder>(builder => {
            builder.RegisterAssemblyTypes(typeof(Program).GetTypeInfo().Assembly)
                    .Where(x => !x.Name.Contains("Decorator"))
                    .AsClosedTypesOf(typeof(IComputationHandler<,>))
                    .AsImplementedInterfaces();

            // decorators are executed in the order they are registered
            builder.RegisterDecorator<TableMappingDecorator,
                IRequestHandler<SearchQuery.Query, IApiResponse>>();

            builder.RegisterDecorator<DecodeGeometryDecorator,
                IRequestHandler<SearchQuery.Query, IApiResponse>>();

            builder.RegisterDecorator<ShapeFieldDecorator,
                IRequestHandler<SearchQuery.Query, IApiResponse>>();

            builder.RegisterDecorator<AttributeTableKeyFormatting.Decorator,
                IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract?>?>>();

            builder.RegisterDecorator<DoubleAvenuesException.Decorator,
                IComputationHandler<ZoneParsing.Computation, Address>>();

            builder.Register(c => new ComputeMediator(c.Resolve<IComponentContext>().Resolve))
                   .AsImplementedInterfaces()
                   .SingleInstance();

            // set up mediatr
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder
                .RegisterAssemblyTypes(typeof(Program).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            var services = new ServiceCollection();

            builder.Populate(services);
        });
    }
    public static void ConfigureHttpClients(this WebApplicationBuilder builder) {
        var retryPolicy = HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .Or<TimeoutRejectedException>() // thrown by Polly's TimeoutPolicy if the inner call times out
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(8);

        AddArcGisClient(builder.Services, retryPolicy, timeoutPolicy, builder.Environment.IsProduction());

        builder.Services.AddHttpClient("health-check", client => client.Timeout = new TimeSpan(0, 0, 5))
                .ConfigurePrimaryHttpMessageHandler(() => {
                    var handler = new HttpClientHandler {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };

                    if (handler.SupportsAutomaticDecompression) {
                        handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    }

                    return handler;
                });

        builder.Services.AddHttpClient("udot", client => {
            client.BaseAddress = new Uri("https://roads.udot.utah.gov/");
            client.Timeout = new TimeSpan(0, 0, 15);
        }).ConfigurePrimaryHttpMessageHandler(() => {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression) {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            return handler;
        }).AddPolicyHandler(retryPolicy)
          .AddPolicyHandler(timeoutPolicy);

        builder.Services.AddHttpClient("national-map", client => {
            client.BaseAddress = new Uri("https://elevation.nationalmap.gov/arcgis/rest/services/3DEPElevation/ImageServer/identify");
            client.Timeout = new TimeSpan(0, 0, 5);
        }).ConfigurePrimaryHttpMessageHandler(() => {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression) {
                handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            return handler;
        }).AddPolicyHandler(retryPolicy)
          .AddPolicyHandler(timeoutPolicy);
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
    public static void ConfigureOpenApi(this WebApplicationBuilder builder) {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => {
            c.EnableAnnotations();
            c.DescribeAllParametersInCamelCase();

            c.DocumentFilter<TrimUrlOperationFilter>();
            c.CustomOperationIds(apiDesc => apiDesc.TryGetMethodInfo(out var methodInfo) ? methodInfo.Name : null);

            c.AddSecurityDefinition("apikey", new OpenApiSecurityScheme {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Query,
                Name = "apiKey",
                Description = "A key gathered from developer.mapserv.utah.gov"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "apikey" },
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Query,
                    Name = "apiKey",
                    Description = "A key acquired from developer.mapserv.utah.gov"
                },
                Array.Empty<string>()
            }
        });

            c.AddServer(new OpenApiServer {
                Url = "https://api.mapserv.utah.gov/api/v1",
                Description = "Base url for the version 1 API"
            });

            c.SwaggerDoc("v1", new OpenApiInfo {
                Version = "v1",
                Title = "UGRC API : OpenAPI Documentation",
                Description = "OpenAPI Documentation",
                Contact = new OpenApiContact {
                    Name = "UGRC",
                    Email = "ugrc-developers@utah.gov",
                    Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov")
                },
                License = new OpenApiLicense {
                    Name = "MIT",
                    Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov/blob/main/LICENSE")
                },
            });

            c.SwaggerDoc("v2", new OpenApiInfo {
                Version = "v2",
                Title = "UGRC API : OpenAPI Documentation",
                Description = "OpenAPI Documentation",
                Contact = new OpenApiContact {
                    Name = "UGRC",
                    Email = "ugrc-developers@utah.gov",
                    Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov")
                },
                License = new OpenApiLicense {
                    Name = "MIT",
                    Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov/blob/main/LICENSE")
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);
        });
    }
    public static void ConfigureVersioning(this WebApplicationBuilder builder)
        => builder.Services.AddApiVersioning(options =>
            options.ReportApiVersions = true).EnableApiVersionBinding();
    public static void ConfigureCors(this WebApplicationBuilder builder)
        => builder.Services.AddCors(options => {
            options.AddDefaultPolicy(
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
}
