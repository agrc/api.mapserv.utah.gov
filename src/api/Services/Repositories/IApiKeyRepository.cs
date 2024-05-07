using ugrc.api.Models;

namespace ugrc.api.Services;
public interface IApiKeyRepository {
    Task<ApiKey> GetKey(string key);
}
