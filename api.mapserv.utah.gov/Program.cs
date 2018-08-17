using System;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace api.mapserv.utah.gov {
    public static class Program {
        public static async Task<int> Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Debug()
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                         .Enrich.FromLogContext()
                         .WriteTo.Console()
                         .CreateLogger();

            try {
                Log.Information("Starting web host");

                var host = CreateWebHostBuilder(args).Build();

                var lookupCache = host.Services.GetService(typeof(ILookupCache)) as ILookupCache;
                await lookupCache.InitializeAsync();

                await host.RunAsync();

                return 0;
            } catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly");

                return 1;
            } finally {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
                                                                                    .UseStartup<Startup>()
                                                                                    .UseSerilog();
    }
}
