using AutoMapper;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Models.ViewModels.Usage;

namespace WebAPI.Dashboard.Resolvers
{
    public class ApiKeyActiveResolver : ValueResolver<StatsPerApiKey.Stats, bool>
    {
        protected override bool ResolveCore(StatsPerApiKey.Stats source)
        {
            return source.Status == ApiKey.KeyStatus.Active;
        }
    }

    public class ApiKeyActiveResolver2 : ValueResolver<UsageViewModel, bool>
    {
        protected override bool ResolveCore(UsageViewModel source)
        {
            return source.Key.ApiKeyStatus == ApiKey.KeyStatus.Active;
        }
    }
}