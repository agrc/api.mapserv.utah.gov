using Microsoft.AspNetCore.Http;

namespace AGRC.api.Filters {
    public interface IKeyProvider {
        string Get(HttpRequest request);
    }

    public interface IBrowserKeyProvider : IKeyProvider {
    }

    public interface IServerIpProvider : IKeyProvider {
    }
}
