using System.Threading.Tasks;
using api.mapserv.utah.gov.Services;
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

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>();
        }
    }
}

