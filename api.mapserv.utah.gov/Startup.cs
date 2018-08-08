using api.mapserv.utah.gov.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
