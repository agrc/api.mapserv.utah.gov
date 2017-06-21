using AutoMapper;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Models.ViewModels.Keys;
using WebAPI.Dashboard.Models.ViewModels.Usage;

namespace WebAPI.Dashboard.Resolvers
{
    public class ApiKeyActiveResolver : IValueResolver<StatsPerApiKey.Stats, ApiKeyViewModel, bool>
    {
        public bool Resolve(StatsPerApiKey.Stats source, ApiKeyViewModel destination, bool member, ResolutionContext context)
        {
            return source.Status == ApiKey.KeyStatus.Active;
        }
    }

    public class ApiKeyActiveResolver2 : IValueResolver<UsageViewModel, ApiKeyViewModel, bool>
    {
        public bool Resolve(UsageViewModel source, ApiKeyViewModel destination, bool member, ResolutionContext context)
        {
            return source.Key.ApiKeyStatus == ApiKey.KeyStatus.Active;
        }
    }
}