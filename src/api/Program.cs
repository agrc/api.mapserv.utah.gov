using System;
using System.IO;
using System.Threading.Tasks;
using AGRC.api.Cache;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.GoogleCloudLogging;

namespace AGRC.api {
    public static class Program {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args) {
            var config = new GoogleCloudLoggingSinkOptions {
                LogName = "api.mapserv.utah.gov",
                UseSourceContextAsLogName = false,
                ResourceType = "global",
                ServiceName = "api.mapserv.utah.gov",
                ServiceVersion = "1.12.2",
                ProjectId = "ut-dts-agrc-web-api-dv"
            };

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (environment == Environments.Development) {
                const string projectId = "ut-dts-agrc-web-api-dv";
                const string fileName = "ut-dts-agrc-web-api-dv-log-writer.json";

                config.ProjectId = projectId;

                try {
                    config.GoogleCredentialJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName));
                } catch (FileNotFoundException) {
                    // use the GOOGLE_APPLICATION_CREDENTIALS
                }
            }

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                // .WriteTo.GoogleCloudLogging(config)
                .CreateLogger();

            try {
                logger.Information("Starting web host");

                var host = CreateHostBuilder(args).Build();

                var lookupCache = host.Services.GetService(typeof(ILookupCache)) as ILookupCache;
                await lookupCache.InitializeAsync();

                logger.Information("Completed");

                await host.RunAsync();

                return 0;
            } catch (Exception ex) {
                logger.Fatal(ex, "Host terminated unexpectedly");

                return 1;
            } finally {
                logger.Information("Shutting down");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => {
                builder.UseStartup<Startup>();
                builder.UseConfiguration(Configuration);
                builder.ConfigureLogging(x => x.ClearProviders());
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .UseSerilog((context, config) => {
                var googleConfig = new GoogleCloudLoggingSinkOptions {
                    LogName = "api.mapserv.utah.gov",
                    UseSourceContextAsLogName = false,
                    ResourceType = "global",
                    ServiceName = "api.mapserv.utah.gov",
                    ServiceVersion = "1.12.2",
                    ProjectId = "ut-dts-agrc-web-api-dv"
                };

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (environment == Environments.Development) {
                    const string projectId = "ut-dts-agrc-web-api-dv";
                    const string fileName = "ut-dts-agrc-web-api-dv-log-writer.json";
                    googleConfig.ProjectId = projectId;

                    try {
                        googleConfig.GoogleCredentialJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName));
                    } catch (FileNotFoundException) {
                        // use the GOOGLE_APPLICATION_CREDENTIALS
                    }
                }

                config.ReadFrom.Configuration(context.Configuration);
                // config.WriteTo.GoogleCloudLogging(googleConfig);
            });
    }
}
