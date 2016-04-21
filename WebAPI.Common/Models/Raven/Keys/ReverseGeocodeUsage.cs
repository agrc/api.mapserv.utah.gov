namespace WebAPI.Common.Models.Raven.Keys
{
    public class ReverseGeocodeUsage : KeyUsageBase
    {
        public double X { get; set; }
        public double Y { get; set; }

        public ReverseGeocodeUsage(string apiKeyId, string accountId, long lastUsedTicks, double x, double y) : base(apiKeyId, accountId, lastUsedTicks)
        {
            X = x;
            Y = y;
        }
    }
}