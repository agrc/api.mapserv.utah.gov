using System;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Commands;
using api.mapserv.utah.gov.Models.Options;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace api.mapserv.utah.gov
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddApiVersioning(x =>
            {
                x.ReportApiVersions = true;
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.Configure<CredentialOptions>(Configuration);

// TODO add when out of preview (2.1)
//            services.AddHttpClient("SomeCustomAPI", client =>
//            {
//                client.BaseAddress = new Uri("locator url");
//            });

            services.AddSingleton<IRegexCache, RegexCache>();
            services.AddSingleton<IAbbreviations, Abbreviations>();
            services.AddSingleton<IGoogleDriveCache, GoogleDriveCache>();
            services.AddSingleton(serviceProvider =>
            {
                var secret = serviceProvider.GetService<IOptions<CredentialOptions>>();

                return GoogleCredential.FromJson(secret.Value.Json);
            });

            services.AddTransient<ParseAddressCommand, ParseAddressCommand>();
            services.AddTransient<ParseZoneCommand, ParseZoneCommand>();
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
