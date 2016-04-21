namespace WebAPI.Common.Models.Raven.Keys
{
    public class MultipleGeocodeUsage : GeocodeStreetZoneUsage
    {
        public MultipleGeocodeUsage(string apiKeyId, string accountId, long lastUsedTicks, string inputAddress,
                                    double score, string page)
            : base(apiKeyId, accountId, lastUsedTicks, inputAddress, score)
        {
            Page = page;
        }

        public string Page { get; set; }
    }
}