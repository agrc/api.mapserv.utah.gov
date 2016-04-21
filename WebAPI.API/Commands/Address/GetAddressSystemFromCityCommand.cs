using System.Collections.Generic;
using WebAPI.Common.Abstractions;
using WebAPI.Domain.Linkers;

namespace WebAPI.API.Commands.Address
{
    public class GetAddressSystemFromCityCommand : Command<List<GridLinkable>>
    {
        private readonly string _cityKey;

        public GetAddressSystemFromCityCommand(string cityKey)
        {
            _cityKey = cityKey;
        }
        public override string ToString()
        {
            return string.Format("{0}, CityKey: {1}","GetAddressSystemFromCityCommand", _cityKey);
        }

        protected override void Execute()
        {
            if (string.IsNullOrEmpty(_cityKey))
            {
                Result = null;
                return;
            }

            List<GridLinkable> gridLinkables;
            App.PlaceGridLookup.TryGetValue(_cityKey, out gridLinkables );
            
            Result = gridLinkables??new List<GridLinkable>();
        }
    }
}