using AutoMapper;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Models.ViewModels.Keys;
using WebAPI.Dashboard.Models.ViewModels.Usage;
using WebAPI.Dashboard.Resolvers;

namespace WebAPI.Dashboard
{
    public class AutoMapperConfig
    {
        public static void RegisterMaps()
        {
            Mapper.CreateMap<StatsPerApiKey.Stats, ApiKeyViewModel>()
                  .ForMember(x => x.ApiKey, y => y.MapFrom(z => z.Key))
                  .ForMember(x => x.LastUsed,
                             y => y.ResolveUsing<CalculateTimeAgoResolver>().FromMember(m => m.LastUsed))
                  .ForMember(x => x.Active, y => y.ResolveUsing<ApiKeyActiveResolver>())
                  .ForMember(x => x.Id, y => y.Ignore())
                  .ForMember(x => x.Development,
                             y => y.MapFrom(z => z.ApplicationStatus == ApiKey.ApplicationStatus.Development));

            Mapper.CreateMap<StatsPerService.Stats, ServiceStatsViewModel>()
                  .ForMember(x => x.LastUsed,
                             y => y.ResolveUsing<CalculateTimeAgoResolver>().FromMember(m => m.LastUsed))
                  .ForMember(x => x.UsageCount, y => y.ResolveUsing<UsageFormatterResolver>())
                  .ForMember(x => x.Name, y => y.ResolveUsing<ApiNameResolver>().FromMember(m => m.UsageType));

            Mapper.AssertConfigurationIsValid();
        }
    }
}