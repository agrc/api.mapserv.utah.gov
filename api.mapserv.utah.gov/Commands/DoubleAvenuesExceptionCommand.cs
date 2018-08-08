using System.Linq;
using System.Text.RegularExpressions;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using Serilog;

namespace api.mapserv.utah.gov.Commands
{
  public class DoubleAvenuesExceptionCommand : Command<GeocodeAddress>
  {
      private GeocodeAddress _address;
      private string _city;
      private readonly Regex _ordinal;

      public DoubleAvenuesExceptionCommand(IRegexCache cache)
      {
          _ordinal = cache.Get("avesOrdinal");
      }

      public void Initialize(GeocodeAddress address, string city)
      {
          _address = address;
          _city = city ?? "";
          _city = _city.Trim();
      }

      public override string ToString() => $"DoubleAvenuesExceptionCommand, zone: {_address.Zip5}, prefix: {_address.PrefixDirection}";

      protected override void Execute()
      {
          // only avenue addresses with no prefix are affected
          if (_address.PrefixDirection != Direction.None || _address.StreetType != StreetType.Avenue || !IsOrdinal(_address.StreetName))
          {
              Log.Debug("Only avenue addresses with no prefix are affected. skipping {address}", _address);
              Result = _address;

              return;
          }

          Log.Debug("Possible double avenues exception {address}, {city}", _address, _city);

          // it's in the problem area in midvale
          const int midvale = 84047;
          if (!string.IsNullOrEmpty(_city) && _city.ToUpperInvariant().Contains("MIDVALE") ||
              _address.Zip5.HasValue && _address.Zip5.Value == midvale)
          {
              Log.Information("Midvale avenues exception, updating {_address} to include West", _address);

              _address.PrefixDirection = Direction.West;
              Result = _address;

              return;
          }

          // update the slc avenues to have an east
          if (_address.AddressGrids.Select(x => x.Grid).Contains("SALT LAKE CITY"))
          {
              Log.Information("SLC avenues exception, updating {_address} to include East", _address);

              _address.PrefixDirection = Direction.East;
              Result = _address;

              return;
          }

          Log.Debug("Not a double avenues exception", _address, _city);

          Result = _address;
      }

      private bool IsOrdinal(string streetname)
      {
          if (string.IsNullOrWhiteSpace(streetname))
          {
              return false;
          }

          streetname = streetname.Replace(" ", string.Empty).Trim();

          return _ordinal.IsMatch(streetname);
      }
  }
}
