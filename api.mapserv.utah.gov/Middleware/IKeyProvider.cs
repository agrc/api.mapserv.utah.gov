using Microsoft.AspNetCore.Http;

namespace api.mapserv.utah.gov.Middleware
{
    public interface IKeyProvider
    {
        string Get(HttpRequest request);
    }

    public interface IBrowserKeyProvider : IKeyProvider { }
    public interface IServerIpProvider : IKeyProvider { }
}