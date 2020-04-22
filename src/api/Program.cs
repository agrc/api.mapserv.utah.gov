using System;
using System.IO;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace api.mapserv.utah.gov {
    public static class Program {

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args) {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            try {
                logger.Information("Starting web host");

                var host = CreateHostBuilder(args).Build();

                var lookupCache = host.Services.GetService(typeof(ILookupCache)) as ILookupCache;
                await lookupCache.InitializeAsync();

                await host.RunAsync();

                return 0;
            } catch (Exception ex) {
                logger.Fatal(ex, "Host terminated unexpectedly");

                return 1;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(builder => {
                builder.UseStartup<Startup>();
                builder.UseConfiguration(Configuration);
                builder.ConfigureLogging(x => x.ClearProviders().AddSerilog());
            });
    }
}
