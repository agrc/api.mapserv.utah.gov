namespace WebAPI.Common.Models.Raven.Keys
{
    public class GeocodeStreetZoneUsage : KeyUsageBase
    {
        public GeocodeStreetZoneUsage(string apiKeyId, string accountId, long lastUsedTicks, string inputAddress,
                                      double score)
            : base(apiKeyId, accountId, lastUsedTicks)
        {
            InputAddress = inputAddress;
            Score = score;
        }

        public string InputAddress { get; set; }
        public double Score { get; set; }
    }
}