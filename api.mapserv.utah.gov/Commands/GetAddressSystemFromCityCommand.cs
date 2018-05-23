using System.Collections.Generic;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;

namespace api.mapserv.utah.gov.Commands
{
  public class GetAddressSystemFromCityCommand : Command<List<GridLinkable>>
  {
      private readonly IGoogleDriveCache _driveCache;
      private string _cityKey;

      public GetAddressSystemFromCityCommand(IGoogleDriveCache driveCache)
      {
          _driveCache = driveCache;
      }

      public void Initialize(string cityKey)
      {
          _cityKey = cityKey;
      }
      public override string ToString() => $"GetAddressSystemFromCityCommand, CityKey: {_cityKey}";

      protected override void Execute()
      {
          if (string.IsNullOrEmpty(_cityKey))
          {
              Result = null;
              return;
          }

            _driveCache.PlaceGrids.TryGetValue(_cityKey, out List<GridLinkable> gridLinkables);

            Result = gridLinkables ?? new List<GridLinkable>();
      }
  }
}
