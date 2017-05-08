using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Indexes
{
    public class StatsPerApiKey
    {
        public class Stats
        {
            public string AccountId { get; set; }
            public string ApiKeyId { get; set; }
            public string Key { get; set; }
            public long UsageCount { get; set; }
            public long LastUsed { get; set; }
            public string Pattern { get; set; }
            public ApiKey.KeyStatus Status { get; set; }
            public ApiKey.ApplicationType Type { get; set; }
            public ApiKey.ApplicationStatus ApplicationStatus { get; set; }

            public Stats()
            {
                
            }

            public Stats(ApiKey key)
            {
                AccountId = key.AccountId;
                Key = key.Key;
                Pattern = key.Pattern;
                Status = key.ApiKeyStatus;
                Type = key.Type;
                ApplicationStatus = key.AppStatus;
            }
        }
    }
}