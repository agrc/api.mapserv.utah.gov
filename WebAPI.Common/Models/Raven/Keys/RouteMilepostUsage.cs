namespace WebAPI.Common.Models.Raven.Keys
{
    public class RouteMilepostUsage : KeyUsageBase
    {
        public string MatchRoute { get; set; }
        public string InputRoute { get; set; }

        public RouteMilepostUsage(string apiKeyId, string accountId, long lastUsedTicks, string matchRoute, string inputRoute)
            : base(apiKeyId, accountId, lastUsedTicks)
        {
            MatchRoute = matchRoute;
            InputRoute = inputRoute;
        }
    }
}