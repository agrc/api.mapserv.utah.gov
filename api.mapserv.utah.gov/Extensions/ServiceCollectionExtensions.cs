using System;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Commands;
using api.mapserv.utah.gov.Filters;
using api.mapserv.utah.gov.Models.SecretOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace api.mapserv.utah.gov.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseOptions(this IServiceCollection services, IConfiguration config){
            services.Configure<GisServerConfiguration>(config);
            services.Configure<GeometryServiceConfiguration>(config);
            services.Configure<DbConfiguration>(config);
        }

        public static void UseDi(this IServiceCollection services)
        {
            services.AddHttpClient("default", client =>
            {
                //var httpClientHandler = new HttpClientHandler();
                //if (httpClientHandler.SupportsAutomaticDecompression)
                //{
                //    httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                //}

                client.Timeout = new TimeSpan(0, 0, 15);
            });

            services.AddSingleton<IAbbreviations, Abbreviations>();
            services.AddSingleton<IRegexCache, RegexCache>();
            services.AddSingleton<IGoogleDriveCache, GoogleDriveCache>();
            services.AddSingleton<IBrowserKeyProvider, AuthorizeApiKeyFromRequest.BrowserKeyProvider>();
            services.AddSingleton<IServerIpProvider, AuthorizeApiKeyFromRequest.ServerIpProvider>();
            services.AddSingleton<AuthorizeApiKeyFromRequest>();

            services.AddTransient<ParseAddressCommand>();
            services.AddTransient<ParseZoneCommand>();
            services.AddTransient<GetLocatorsForAddressCommand>();
            services.AddTransient<LocatePoBoxCommand>();
            services.AddTransient<UspsDeliveryPointCommand>();
            services.AddTransient<ReprojectPointsCommand>();
        }
    }
}
