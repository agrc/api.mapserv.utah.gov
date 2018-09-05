using Microsoft.AspNetCore.Http;

namespace api.mapserv.utah.gov.Filters {
    public interface IKeyProvider {
        string Get(HttpRequest request);
    }

    public interface IBrowserKeyProvider : IKeyProvider {
    }

    public interface IServerIpProvider : IKeyProvider {
    }
}
