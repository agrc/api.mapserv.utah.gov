using AutoMapper;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Resolvers
{
    public class ApiKeyActiveResolver : ValueResolver<StatsPerApiKey.Stats, bool>
    {
        protected override bool ResolveCore(StatsPerApiKey.Stats source)
        {
            return source.Status == ApiKey.KeyStatus.Active;
        }
    }
}