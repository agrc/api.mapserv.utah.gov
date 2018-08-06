using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Services
{
    public class PostmanApiKeyRepository : IApiKeyRepository
    {
        public Task<ApiKey> GetKey (string key)
        {
            if (key.ToLowerInvariant() == "postman")
            {
                return Task.FromResult(new ApiKey(key)
                {
                    Enabled = ApiKey.KeyStatus.Active,
                    Configuration = ApiKey.ApplicationStatus.Production,
                    Deleted = false,
                    IsMachineName = false,
                    RegexPattern = "localhost",
                    Type = ApiKey.ApplicationType.Browser,
                    Whitelisted = true
                });
            }

            if (key.ToLowerInvariant() == "postman-disabled")
            {
                return Task.FromResult(new ApiKey(key)
                {
                    Enabled = ApiKey.KeyStatus.Disabled,
                    Configuration = ApiKey.ApplicationStatus.Production,
                    Deleted = false,
                    IsMachineName = false,
                    RegexPattern = "localhost",
                    Type = ApiKey.ApplicationType.Browser
                });
            }

            if (key.ToLowerInvariant() == "postman-deleted")
            {
                return Task.FromResult(new ApiKey(key)
                {
                    Enabled = ApiKey.KeyStatus.Active,
                    Configuration = ApiKey.ApplicationStatus.Production,
                    Deleted = true,
                    IsMachineName = false,
                    RegexPattern = "localhost",
                    Type = ApiKey.ApplicationType.Browser
                });
            }

            if (key.ToLowerInvariant() == "postman-ip")
            {
                return Task.FromResult(new ApiKey(key)
                {
                    Enabled = ApiKey.KeyStatus.Active,
                    Configuration = ApiKey.ApplicationStatus.Production,
                    Deleted = false,
                    IsMachineName = false,
                    Pattern = "::1",
                    Type = ApiKey.ApplicationType.Server
                });
            }

            return Task.FromResult<ApiKey>(null);
        }
    }
}
