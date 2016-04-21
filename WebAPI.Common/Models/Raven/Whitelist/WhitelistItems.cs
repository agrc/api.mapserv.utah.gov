using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Common.Models.Raven.Whitelist
{
    public class WhitelistItems
    {
        public WhitelistItems(string key, ApiKey.KeyStatus status)
        {
            Key = key;
            Status = status;
        }

        public string Key { get; set; }
        public ApiKey.KeyStatus Status { get; set; }
    }
}