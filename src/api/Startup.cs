using System;
using System.IO;
using System.Reflection;
using api.mapserv.utah.gov.Extensions;
using api.mapserv.utah.gov.Features.Health;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using WebApiContrib.Core.Formatter.Jsonp;

namespace api.mapserv.utah.gov {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddCors();
            services.AddMvc(options => {
                        options.AddApiResponseFormatters();
                        options.AddJsonpOutputFormatter();
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
            services.UseOptions(Configuration);
            services.UseDi();

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

        public void ConfigureContainer(ContainerBuilder builder) {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            var mediatrOpenTypes = new[]
            {
                typeof(IRequestHandler<,>),
                typeof(INotificationHandler<>),
            };

            foreach (var mediatrOpenType in mediatrOpenTypes) {
                builder
                    .RegisterAssemblyTypes(typeof(Startup).GetTypeInfo().Assembly)
                    .AsClosedTypesOf(mediatrOpenType)
                    .AsImplementedInterfaces();
            }

            builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            builder.Register<ServiceFactory>(ctx => {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/api.json"; });

            app.UseSwaggerUI(c => {
                c.DocumentTitle = "AGRC WebAPI OpenAPI Documentation";
                c.RoutePrefix = "openapi";
                c.SwaggerEndpoint("/openapi/v1/api.json", "v1");
                c.SupportedSubmitMethods();
                c.EnableDeepLinking();
                c.DocExpansion(DocExpansion.List);
            });

            app.UseMvc();
            app.UseHealthChecks("/health/details", new HealthCheckOptions {
                ResponseWriter = HealthCheckResponseWriter.WriteDetailsJson
            });
            app.UseHealthChecks("/health");
        }
    }
}
