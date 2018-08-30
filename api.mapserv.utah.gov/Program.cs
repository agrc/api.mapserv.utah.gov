using System;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace api.mapserv.utah.gov {
    public static class Program {
        public static async Task<int> Main(string[] args) {
            ILogger log = null;
            try {
                var host = CreateWebHostBuilder(args).Build();

                log = host.Services.GetService(typeof(ILogger)) as ILogger;

                log?.Information("Starting web host");

                var lookupCache = host.Services.GetService(typeof(ILookupCache)) as ILookupCache;
                await lookupCache.InitializeAsync();

                await host.RunAsync();

                return 0;
            } catch (Exception ex) {
                log?.Fatal(ex, "Host terminated unexpectedly");

                return 1;
            } 
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) => WebHost.CreateDefaultBuilder(args)
                                                                                    .UseStartup<Startup>()
                                                                                    .UseSerilog();
    }
}
