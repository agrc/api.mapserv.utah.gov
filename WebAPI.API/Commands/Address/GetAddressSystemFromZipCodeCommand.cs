using System.Collections.Generic;
using WebAPI.Common.Abstractions;
using WebAPI.Domain.Linkers;

namespace WebAPI.API.Commands.Address
{
    public class GetAddressSystemFromZipCodeCommand : Command<List<GridLinkable>>
    {
        private readonly string _zip;
        private string _grid;

        public GetAddressSystemFromZipCodeCommand(int? zip)
        {
            if(zip.HasValue)
               _zip = zip.ToString();

            _grid = "none";
        }

        public override string ToString()
        {
            return string.Format("{0}, Zip: {1}, {2}",   "GetAddressSystemFromZipCodeCommand", _zip, _grid);
        }

        protected override void Execute()
        {
            if (string.IsNullOrEmpty(_zip))
            {
                Result = null;
                return;
            }

            List<GridLinkable> gridLinkables;
            App.ZipCodeGridLookup.TryGetValue(_zip, out gridLinkables);

            Result = gridLinkables ?? new List<GridLinkable>();
            _grid = string.Join(",", Result);
        }
    }
}