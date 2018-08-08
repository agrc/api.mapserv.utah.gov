using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace api.mapserv.utah.gov
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            var lookupCache = host.Services.GetService(typeof(ILookupCache)) as ILookupCache;
            await lookupCache.InitializeAsync();

            await host.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>();
        }
    }
}