
using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Areas.admin.Models
{
    public class ApiKeyStats
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

            public ApiKeyStats()
            {
                
            }

            public ApiKeyStats(ApiKey key)
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
