using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AGRC.api.Extensions;
using AGRC.api.Features.Geocoding;
using AGRC.api.Features.GeometryService;
using AGRC.api.Features.Health;
using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using api.OpenApi;
using Autofac;
using CorrelationId;
using CorrelationId.DependencyInjection;
using MediatR;
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
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO.Converters;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AGRC.api {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddDefaultCorrelationId(options => {
                options.AddToLoggingScope = true;
                options.EnforceHeader = false;
                options.IgnoreRequestHeader = true;
                options.IncludeInResponse = false;
                options.UpdateTraceIdentifier = false;
            });

            services.AddCors(options => {
                options.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

            services.AddControllers(options => {
                options.AddApiResponseFormatters();
                // options.AddJsonpOutputFormatter();
            })
            .AddJsonOptions(options => {
                // open api is currently using system.text.json
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
            });

            services.AddApiVersioning(x => {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
                x.Conventions.Add(new VersionByNamespaceConvention());
            });

            services.AddHealthChecks()
                    .AddCheck<StartupHealthCheck>("Startup", failureStatus: HealthStatus.Degraded, tags: new[] { "startup" })
                    .AddCheck<CacheHealthCheck>("Cache", failureStatus: HealthStatus.Degraded, tags: new[] { "health" })
                    .AddCheck<KeyStoreHealthCheck>("KeyStore", failureStatus: HealthStatus.Unhealthy, tags: new[] { "health" })
                    .AddCheck<GeometryServiceHealthCheck>("GeometryService", failureStatus: HealthStatus.Degraded, tags: new[] { "health" })
                    .AddCheck<LocatorHealthCheck>("Geolocators", tags: new[] { "health" });

            services.UseOptions(Configuration);
            services.UseDi(Environment);

            services.AddSwaggerGen(c => {
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

            NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();
        }

        public void ConfigureContainer(ContainerBuilder builder) {
            // set up computations
            builder.RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                    .Where(x => !x.Name.Contains("Decorator"))
                    .AsClosedTypesOf(typeof(IComputationHandler<,>))
                    .AsImplementedInterfaces();

            builder.RegisterDecorator<DoubleAvenuesException.Decorator,
                IComputationHandler<ZoneParsing.Computation, AddressWithGrids>>();

            builder.RegisterDecorator<SqlQuery.ShapeFieldDecorator,
                IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();

            builder.RegisterDecorator<SqlQuery.TableMappingDecorator,
                IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();

            builder.RegisterDecorator<AttributeTableKeyFormatting.Decorator,
                IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();

            builder.RegisterGenericDecorator(typeof(Reproject.Decorator<,>), typeof(IComputationHandler<,>));

            builder.Register(c => new ComputeMediator(c.Resolve<IComponentContext>().Resolve))
                   .AsImplementedInterfaces()
                   .SingleInstance();

            // set up mediatr
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder
                .RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.Register<ServiceFactory>(ctx => {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            app.UseCorrelationId();

            app.UseRouting();

            app.UseCors();

            if (env.IsDevelopment()) {
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

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/api/v1/health/details", new HealthCheckOptions {
                    Predicate = healthCheck => healthCheck.Tags.Contains("health"),
                    ResponseWriter = DetailedHealthCheckResponseWriter.WriteDetailsJson
                });
                endpoints.MapHealthChecks("/api/v1/health", new HealthCheckOptions {
                    Predicate = healthCheck => healthCheck.Tags.Contains("health"),
                });
                endpoints.MapHealthChecks("", new HealthCheckOptions() {
                    Predicate = healthCheck => healthCheck.Tags.Contains("startup"),
                    ResponseWriter = StartupHealthCheckResponseWriter.WriteText
                });
            });
        }
    }
}
