using System.Collections.Generic;
using WebAPI.Domain.Addresses;

namespace WebAPI.Domain.EndpointArgs
{
    public class MultipleGeocodeContainerArgs
    {
        public List<AddressWithId> Addresses { get; set; }
    }
}