using api.mapserv.utah.gov.Models.SecretOptions;
using api.mapserv.utah.gov.Services;
using api.mapserv.utah.gov.Extensions;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger _log;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory logFactory)
        {
            _env = env;
            Configuration = configuration;
            _log = logFactory.CreateLogger("Startup");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc()
                    .AddJsonOptions(options => options.SerializerSettings.NullValueHandling =
                                        NullValueHandling.Ignore);

            services.AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.UseOptions(Configuration);
            services.UseDi();

            if (_env.IsDevelopment())
            {
                _log.LogWarning("Getting credentials from secret manager");
                services.AddSingleton<IApiKeyRepository, PostgreApiKeyRepository>();
                services.Configure<GoogleCredentialConfiguration>(Configuration);

                services.AddSingleton<GoogleCredential>(serviceProvider =>
                {
                    var secret = serviceProvider.GetService<IOptions<GoogleCredentialConfiguration>>();

                    return GoogleCredential.FromJson(secret.Value.Json);
                });
            }

            if (_env.IsEnvironment("DockerDevelopment"))
            {
                _log.LogWarning("Getting credentials from appsecrets");
                services.AddSingleton<IApiKeyRepository, PostgreApiKeyRepository>();
                services.AddSingleton<GoogleCredential>(serviceProvider =>
                {
                    var secret = Configuration["json"];

                    return GoogleCredential.FromJson(secret);
                });
            }

            if (_env.IsStaging())
            {
                services.AddSingleton<GoogleCredential>(serviceProvider =>
                {
                    _log.LogWarning("Getting credentials from appsecrets");
                    var secret = Configuration["json"];

                    return GoogleCredential.FromJson(secret);
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("DockerDevelopment"))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
