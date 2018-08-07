using System.Collections.Generic;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Commands
{
    public class GetAddressSystemFromZipCodeCommand : Command<List<GridLinkable>>
    {
        private readonly ILookupCache _driveCache;
        private string _zip;
        private string _grid;

        public GetAddressSystemFromZipCodeCommand(ILookupCache driveCache)
        {
            _driveCache = driveCache;
        }

        public void Initialize(int? zip)
        {
            if (zip.HasValue)
            {
                _zip = zip.ToString();
            }

            _grid = "none";
        }

        public override string ToString() => $"GetAddressSystemFromZipCodeCommand, Zip: {_zip}, {_grid}";

        protected override void Execute()
        {
            if (string.IsNullOrEmpty(_zip))
            {
                Result = null;

                return;
            }

            _driveCache.ZipCodesGrids.TryGetValue(_zip, out List<GridLinkable> gridLinkables);

            Result = gridLinkables ?? new List<GridLinkable>();
            _grid = string.Join(",", Result);
        }
    }
}
