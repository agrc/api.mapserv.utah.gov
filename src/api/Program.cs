using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            var googleConfig = new GoogleCloudLoggingSinkOptions {
                LogName = "api.mapserv.utah.gov",
                UseSourceContextAsLogName = false,
                ResourceType = "global",
                ServiceName = "api.mapserv.utah.gov",
                ServiceVersion = "1.12.2",
                ProjectId = "ut-dts-agrc-web-api-prod"
            };

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (new[] { Environments.Development, Environments.Staging }.Contains(environment)) {
                const string projectId = "ut-dts-agrc-web-api-dev";
                const string fileName = "ut-dts-agrc-web-api-dev-log-writer.json";

                googleConfig.ProjectId = projectId;

                try {
                    googleConfig.GoogleCredentialJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName));
                } catch (FileNotFoundException) {
                    // use the GOOGLE_APPLICATION_CREDENTIALS
                }
            }

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.GoogleCloudLogging(googleConfig)
                .CreateLogger();

            try {
                logger.Information("Starting web host");

                var host = CreateHostBuilder(args).Build();

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
            .ConfigureAppConfiguration((hostingContext, config) => {
                if (!hostingContext.HostingEnvironment.IsDevelopment()) {
                    config.AddJsonFile(
                        Path.Combine(Path.DirectorySeparatorChar.ToString(), "secrets", "dotnet", "appsettings.Production.json"),
                        optional: false,
                        reloadOnChange: true
                    );
                }
            })
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
                    ProjectId = "ut-dts-agrc-web-api-prod"
                };

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                if (new[] { Environments.Development, Environments.Staging }.Contains(environment)) {
                    const string projectId = "ut-dts-agrc-web-api-dev";
                    const string fileName = "ut-dts-agrc-web-api-dev-log-writer.json";
                    googleConfig.ProjectId = projectId;

                    try {
                        googleConfig.GoogleCredentialJson = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), fileName));
                    } catch (FileNotFoundException) {
                        // use the GOOGLE_APPLICATION_CREDENTIALS
                    }
                }

                config.ReadFrom.Configuration(context.Configuration);
                config.WriteTo.GoogleCloudLogging(googleConfig);
            });
    }
}
