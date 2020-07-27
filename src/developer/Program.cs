using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace developer.mapserv.utah.gov {
    public class Program
    {
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => {
                builder.UseStartup <Startup>();
            });
    }
}
