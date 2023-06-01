using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using AGRC.api.Extensions;
using AGRC.api.Features.Geocoding;
using AGRC.api.Features.GeometryService;
using AGRC.api.Features.Health;
using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Quirks;
using api.OpenApi;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CorrelationId;
using CorrelationId.DependencyInjection;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO.Converters;
using Serilog.Sinks.GoogleCloudLogging;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("starting web host");

try {
    var builder = WebApplication.CreateBuilder(args);

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

    builder.Logging.ClearProviders();
    builder.Host
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .UseSerilog((context, provider) => {
            provider.ReadFrom.Configuration(context.Configuration);

            if (!context.HostingEnvironment.IsDevelopment()) {
                provider.WriteTo.GoogleCloudLogging(new GoogleCloudLoggingSinkOptions(
                    context.Configuration["GoogleCloudLogging:projectId"],
                    "global",
                    context.Configuration["GoogleCloudLogging:logName"],
                    new Dictionary<string, string> {
                        { "configuration", context.HostingEnvironment.EnvironmentName }
                    },
                    null,
                    false,
                    true,
                    null,
                    context.Configuration["GoogleCloudLogging:serviceName"],
                    context.Configuration["GoogleCloudLogging:serviceVersion"]
                ));
            }
        });

    builder.Services.AddDefaultCorrelationId(options => {
        options.AddToLoggingScope = true;
        options.EnforceHeader = false;
        options.IgnoreRequestHeader = true;
        options.IncludeInResponse = false;
        options.UpdateTraceIdentifier = false;
    });
    builder.Services.AddCors(options => {
        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
    });
    builder.Services.AddControllers(options => {
        options.AddApiResponseFormatters();
        // TODO? is this needed?
        // options.AddJsonpOutputFormatter();
    })
    .AddJsonOptions(options => {
        // open api is currently using system.text.json
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    });

    builder.Services.AddApiVersioning(x => {
        x.ReportApiVersions = true;
        x.AssumeDefaultVersionWhenUnspecified = true;
        x.DefaultApiVersion = new ApiVersion(1, 0);
        x.Conventions.Add(new VersionByNamespaceConvention());
    });

    builder.Services.AddHealthChecks()
        .AddCheck<StartupHealthCheck>("Startup", failureStatus: HealthStatus.Degraded, tags: new[] { "startup" })
        .AddCheck<CacheHealthCheck>("Cache", failureStatus: HealthStatus.Degraded, tags: new[] { "health" })
        .AddCheck<KeyStoreHealthCheck>("KeyStore", failureStatus: HealthStatus.Unhealthy, tags: new[] { "health" })
        .AddCheck<GeometryServiceHealthCheck>("ArcGIS:GeometryService", failureStatus: HealthStatus.Degraded, tags: new[] { "health" })
        .AddCheck<UdotServiceHealthCheck>("ArcGIS:RoadsAndHighwaysService", failureStatus: HealthStatus.Degraded, tags: new[] { "health" })
        .AddCheck<LocatorHealthCheck>("ArcGIS:LocatorServices", tags: new[] { "health" })
        .AddCheck<BigQueryHealthCheck>("Database", tags: new[] { "health" });

    builder.Services.UseOptions(builder.Configuration);
    builder.Services.UseDi(builder.Environment);

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
            Title = "UGRC WebAPI : OpenAPI Documentation",
            Description = "OpenAPI Documentation",
            Contact = new OpenApiContact {
                Name = "UGRC",
                Email = "sgourley@utah.gov",
                Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov")
            },
            License = new OpenApiLicense {
                Name = "MIT",
                Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov/blob/master/LICENSE")
            },
        });

        c.SwaggerDoc("v2", new OpenApiInfo {
            Version = "v2",
            Title = "UGRC WebAPI : OpenAPI Documentation",
            Description = "OpenAPI Documentation",
            Contact = new OpenApiContact {
                Name = "UGRC",
                Email = "sgourley@utah.gov",
                Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov")
            },
            License = new OpenApiLicense {
                Name = "MIT",
                Url = new Uri("https://github.com/agrc/api.mapserv.utah.gov/blob/master/LICENSE")
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        c.IncludeXmlComments(xmlPath);
    });

    builder.Host.ConfigureContainer<ContainerBuilder>(builder => {
        builder.RegisterAssemblyTypes(typeof(Program).GetTypeInfo().Assembly)
                .Where(x => !x.Name.Contains("Decorator"))
                .AsClosedTypesOf(typeof(IComputationHandler<,>))
                .AsImplementedInterfaces();

        builder.RegisterDecorator<DoubleAvenuesException.Decorator,
            IComputationHandler<ZoneParsing.Computation, Address>>();

        builder.RegisterDecorator<SqlQuery.ShapeFieldDecorator,
            IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract?>?>>();

        builder.RegisterDecorator<SqlQuery.TableMappingDecorator,
            IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract?>?>>();

        builder.RegisterDecorator<AttributeTableKeyFormatting.Decorator,
            IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract?>?>>();

        builder.RegisterGenericDecorator(typeof(Reproject.Decorator<,>), typeof(IComputationHandler<,>));

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

    var app = builder.Build();

    app.UseCorrelationId();

    app.UseRouting();

    if (app.Environment.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
    }

    app.UseSwagger(c => c.RouteTemplate = "openapi/{documentName}/api.json");

    app.UseSwaggerUI(c => {
        c.DocumentTitle = "UGRC WebAPI OpenAPI Documentation";
        c.RoutePrefix = "openapi";
        c.SwaggerEndpoint("/openapi/v1/api.json", "v1");
        c.SwaggerEndpoint("/openapi/v2/api.json", "v2");
        c.SupportedSubmitMethods();
        c.EnableDeepLinking();
        c.DocExpansion(DocExpansion.List);
    });

    app.MapControllers();

    app.UseMiddleware<JsonpMiddleware>();

    app.MapHealthChecks("/api/v1/health/details", new HealthCheckOptions {
        Predicate = healthCheck => healthCheck.Tags.Contains("health"),
        ResponseWriter = DetailedHealthCheckResponseWriter.WriteDetailsJson
    });

    app.MapHealthChecks("/api/v1/health", new HealthCheckOptions {
        Predicate = healthCheck => healthCheck.Tags.Contains("health"),
    });

    app.MapHealthChecks("", new HealthCheckOptions() {
        Predicate = healthCheck => healthCheck.Tags.Contains("startup"),
        ResponseWriter = StartupHealthCheckResponseWriter.WriteText
    });

    logger.Information("program configuration completed");
    app.Run();
} catch (Exception ex) {
    logger.Fatal(ex, "host terminated unexpectedly");
} finally {
    logger.Information("shutting down");
    Log.CloseAndFlush();
}
