using System;
using System.Net;
using System.Net.Http;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Commands;
using api.mapserv.utah.gov.Models.SecretOptions;
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
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);

            services.AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.Configure<GisServerConfiguration>(Configuration);
            services.Configure<GeometryServiceConfiguration>(Configuration);

            if (_env.IsStaging())
            {
                services.AddSingleton<GoogleCredential>(serviceProvider =>
                {
                    _log.LogWarning("Getting credentials from appsecrets");
                    var secret = Configuration["json"].ToString();

                    return GoogleCredential.FromJson(secret);
                });
            }
            else
            {
                _log.LogWarning("Getting credentials from secrets");
                services.Configure<GoogleCredentialConfiguration>(Configuration);

                services.AddSingleton<GoogleCredential>(serviceProvider =>
                {
                    var secret = serviceProvider.GetService<IOptions<GoogleCredentialConfiguration>>();

                    return GoogleCredential.FromJson(secret.Value.Json);
                });
            }

// TODO add when out of preview (2.1)
//            services.AddHttpClient("SomeCustomAPI", client =>
//            {
//                client.BaseAddress = new Uri("locator url");
//            });            services.AddSingleton<IRegexCache, RegexCache>();

            services.AddSingleton<HttpClient>(serviceProvider =>
            {
                var httpClientHandler = new HttpClientHandler();
                if (httpClientHandler.SupportsAutomaticDecompression)
                {
                    httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                }

                return new HttpClient(httpClientHandler)
                {
                    Timeout = new TimeSpan(0, 0, 60)
                };
            });

            services.AddSingleton<IAbbreviations, Abbreviations>();
            services.AddSingleton<IRegexCache, RegexCache>();
            services.AddSingleton<IGoogleDriveCache, GoogleDriveCache>();

            services.AddTransient<ParseAddressCommand, ParseAddressCommand>();
            services.AddTransient<ParseZoneCommand, ParseZoneCommand>();
            services.AddTransient<GetLocatorsForAddressCommand, GetLocatorsForAddressCommand>();
            services.AddTransient<LocatePoBoxCommand, LocatePoBoxCommand>();
            services.AddTransient<ReprojectPointsCommand, ReprojectPointsCommand>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
