using AGRC.api.Models;

#nullable enable
namespace AGRC.api.Services;
public class PostmanApiKeyRepository : IApiKeyRepository {
    public Task<ApiKey> GetKey(string key) {
        if (string.Equals(key, "postman", StringComparison.InvariantCultureIgnoreCase)) {
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

        if (string.Equals(key, "postman-disabled", StringComparison.InvariantCultureIgnoreCase)) {
            return Task.FromResult(new ApiKey(key) {
                Enabled = ApiKey.KeyStatus.Disabled,
                Configuration = ApiKey.ApplicationStatus.Production,
                Deleted = false,
                IsMachineName = false,
                RegexPattern = "localhost",
                Type = ApiKey.ApplicationType.Browser
            });
        }

        if (string.Equals(key, "postman-deleted", StringComparison.InvariantCultureIgnoreCase)) {
            return Task.FromResult(new ApiKey(key) {
                Enabled = ApiKey.KeyStatus.Active,
                Configuration = ApiKey.ApplicationStatus.Production,
                Deleted = true,
                IsMachineName = false,
                RegexPattern = "localhost",
                Type = ApiKey.ApplicationType.Browser
            });
        }

        if (string.Equals(key, "postman-ip", StringComparison.InvariantCultureIgnoreCase)) {
            return Task.FromResult(new ApiKey(key) {
                Enabled = ApiKey.KeyStatus.Active,
                Configuration = ApiKey.ApplicationStatus.Production,
                Deleted = false,
                IsMachineName = false,
                Pattern = "::1",
                Type = ApiKey.ApplicationType.Server
            });
        }

        throw new ArgumentException("Invalid key");
    }
}
