using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Services
{
    public interface IApiKeyRepository
    {
        Task<ApiKey> GetKey(string key);
    }
}