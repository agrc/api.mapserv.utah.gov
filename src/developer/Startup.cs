using developer.mapserv.utah.gov.Models.Configuration;
using developer.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Microsoft.Extensions.Hosting;

namespace developer.mapserv.utah.gov
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
            services.Configure<PepperModel>(Configuration);
            services.Configure<DatabaseConfiguration>(Configuration);

            services.AddControllersWithViews();

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            services.AddScoped<NpgsqlConnection>((serviceProvider) =>
            {
                var dbOptions = serviceProvider.GetService<IOptions<DatabaseConfiguration>>();

                var conn = new NpgsqlConnection(dbOptions.Value.ConnectionString);
                conn.Open();

                return conn;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => {
                        options.LoginPath = new PathString("/accountaccess/");
                        options.AccessDeniedPath = new PathString("/accountaccess/");
                        options.SlidingExpiration = true;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("DockerDevelopment"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            app.UseCookiePolicy();
        }
    }
}
