using AGRC.api.Models;

namespace AGRC.api.Services;
public class PostmanApiKeyRepository : IApiKeyRepository {
    public Task<ApiKey> GetKey(string key) {
        if (string.Equals(key, "postman", StringComparison.InvariantCultureIgnoreCase)) {
            return Task.FromResult(new ApiKey(key) {
                Flags = new() { { "deleted", false }, { "disabled", false }, { "server", false }, { "production", true } },
                IsMachineName = false,
                RegularExpression = "localhost",
                Elevated = true
            });
        }

        if (string.Equals(key, "postman-disabled", StringComparison.InvariantCultureIgnoreCase)) {
            return Task.FromResult(new ApiKey(key) {
                Flags = new() { { "deleted", false }, { "disabled", true }, { "server", false }, { "production", true } },
                IsMachineName = false,
                RegularExpression = "localhost",
            });
        }

        if (string.Equals(key, "postman-deleted", StringComparison.InvariantCultureIgnoreCase)) {
            return Task.FromResult(new ApiKey(key) {
                Flags = new() { { "deleted", true }, { "disabled", false }, { "server", false }, { "production", true } },
                IsMachineName = false,
                RegularExpression = "localhost",
            });
        }

        if (string.Equals(key, "postman-ip", StringComparison.InvariantCultureIgnoreCase)) {
            return Task.FromResult(new ApiKey(key) {
                Flags = new() { { "deleted", false }, { "disabled", false }, { "server", true }, { "production", true } },
                IsMachineName = false,
                Pattern = "::1",
            });
        }

        throw new ArgumentException("Invalid key");
    }
}
