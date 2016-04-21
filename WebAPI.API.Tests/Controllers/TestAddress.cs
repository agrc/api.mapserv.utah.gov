using WebAPI.Domain.Addresses;
using WebAPI.Domain.ArcServerResponse.Geolocator;

namespace WebAPI.API.Tests.Controllers
{
    public class TestAddress : AddressWithId
    {
        public int KnownScore { get; set; }
        public Location KnownLocation { get; set; }
    }
}