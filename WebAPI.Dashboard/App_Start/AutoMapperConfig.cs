using System;
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

            Mapper.CreateMap<UsageViewModel, ApiKeyViewModel>()
                .ForMember(x => x.ApiKey, y => y.MapFrom(z => z.Key.Key))
                .ForMember(x => x.LastUsed,
                    y => y.ResolveUsing<CalculateTimeAgoResolver>().FromMember(m => m.LastUsedTicks))
                .ForMember(x => x.UsageCount, y => y.MapFrom(z => z.TotalUsageCount))
                .ForMember(x =>x.Type, y=> y.MapFrom(z => z.Key.Type))
                .ForMember(x => x.Active, y => y.ResolveUsing<ApiKeyActiveResolver2>())
                .ForMember(x => x.Id, y => y.Ignore())
                .ForMember(x => x.Development,
                    y => y.MapFrom(z => z.Key.AppStatus == ApiKey.ApplicationStatus.Development))
                .ForMember(x => x.AccountId, y => y.Ignore())
                .ForMember(x => x.Pattern, y => y.MapFrom(z => z.Key.Pattern));

            Mapper.AssertConfigurationIsValid();
        }
    }
}