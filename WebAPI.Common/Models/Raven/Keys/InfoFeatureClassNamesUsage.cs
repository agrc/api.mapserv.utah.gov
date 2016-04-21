namespace WebAPI.Common.Models.Raven.Keys
{
    public class InfoFeatureClassNamesUsage : KeyUsageBase
    {
        public InfoFeatureClassNamesUsage(string apiKeyId, string accountId, long lastUsedTicks)
            : base(apiKeyId, accountId, lastUsedTicks) {}
    }
}