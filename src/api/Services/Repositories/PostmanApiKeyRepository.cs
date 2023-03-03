using System.Threading.Tasks;
using AGRC.api.Models;

namespace AGRC.api.Services {
    public class PostmanApiKeyRepository : IApiKeyRepository {
        public Task<ApiKey> GetKey(string key) {
            if (string.Equals(key, "postman", System.StringComparison.InvariantCultureIgnoreCase)) {
                return Task.FromResult(new ApiKey(key) {
                    Enabled = ApiKey.KeyStatus.Active,
                    Configuration = ApiKey.ApplicationStatus.Production,
                    Deleted = false,
                    IsMachineName = false,
                    RegexPattern = "localhost",
                    Type = ApiKey.ApplicationType.Browser,
                    Elevated = true
                });
            }

            if (string.Equals(key, "postman-disabled", System.StringComparison.InvariantCultureIgnoreCase)) {
                return Task.FromResult(new ApiKey(key) {
                    Enabled = ApiKey.KeyStatus.Disabled,
                    Configuration = ApiKey.ApplicationStatus.Production,
                    Deleted = false,
                    IsMachineName = false,
                    RegexPattern = "localhost",
                    Type = ApiKey.ApplicationType.Browser
                });
            }

            if (string.Equals(key, "postman-deleted", System.StringComparison.InvariantCultureIgnoreCase)) {
                return Task.FromResult(new ApiKey(key) {
                    Enabled = ApiKey.KeyStatus.Active,
                    Configuration = ApiKey.ApplicationStatus.Production,
                    Deleted = true,
                    IsMachineName = false,
                    RegexPattern = "localhost",
                    Type = ApiKey.ApplicationType.Browser
                });
            }

            if (string.Equals(key, "postman-ip", System.StringComparison.InvariantCultureIgnoreCase)) {
                return Task.FromResult(new ApiKey(key) {
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
