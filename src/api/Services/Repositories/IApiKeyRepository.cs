using AGRC.api.Models;

namespace AGRC.api.Services;
public interface IApiKeyRepository {
    Task<ApiKey> GetKey(string key);
}
