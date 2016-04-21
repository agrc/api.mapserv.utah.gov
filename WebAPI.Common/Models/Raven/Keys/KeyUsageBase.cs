namespace WebAPI.Common.Models.Raven.Keys
{
    public abstract class KeyUsageBase
    {
        protected KeyUsageBase(string apiKeyId, string accountId, long lastUsedTicks)
        {
            ApiKeyId = apiKeyId;
            LastUsedTicks = lastUsedTicks;
            AccountId = accountId;
        }

        public string Id { get; set; }
        public string ApiKeyId { get; set; }
        public long LastUsedTicks { get; set; }
        public string AccountId { get; set; }
    }
}