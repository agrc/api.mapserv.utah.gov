using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;
using WebAPI.Common.Abstractions;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Commands.Geocode.Flags
{
    public class DoubleAvenuesExceptionCommand : Command<GeocodeAddress>
    {
        private readonly GeocodeAddress _address;
        private readonly string _city;
        private readonly Regex _ordinal;

        public DoubleAvenuesExceptionCommand(GeocodeAddress address, string city)
        {
            _address = address;
            _city = city ?? "";
            _city = _city.Trim();
            _ordinal = BuildOridnalRegex();
        }

        private static Regex BuildOridnalRegex()
        {
            // avenues in slc go to 18. 1-8 in midvale
            var ordinals = Enumerable.Range(1, 18)
                .Select(ToOrdinal)
                .Concat(Enumerable.Range(1, 18).Select(x => x.ToString()))
                .Concat(new[]
                {
                    "one", "first",
                    "two", "second",
                    "three", "third",
                    "four", "fourth",
                    "five", "fifth",
                    "six", "sixth",
                    "seven", "seventh",
                    "eight", "eighth",
                    "nine", "ninth",
                    "ten", "tenth",
                    "eleven", "eleventh",
                    "twelve", "twelfth",
                    "thirteen", "thirteenth",
                    "fourteen", "fourteenth",
                    "fifteen", "fifteenth",
                    "sixteen", "sixteenth",
                    "seventeen", "seventeenth",
                    "eighteen", "eighteenth"
                });

            return new Regex(string.Format("^({0})$", string.Join("|", ordinals)), RegexOptions.IgnoreCase);
        }

        public static string ToOrdinal(int number)
        {
            if (number < 0)
            {
                return number.ToString();
            }

            var rem = number % 100;
            if (rem >= 11 && rem <= 13)
            {
                return number + "th";
            }

            switch (number % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        public override string ToString()
        {
            return $"DoubleAvenuesExceptionCommand, zone: {_address.Zip5}, prefix: {_address.PrefixDirection}";
        }

        protected override void Execute()
        {
            // only avenue addresses with no prefix are affected
            if (_address.PrefixDirection != Direction.None || _address.StreetType != StreetType.Avenue || !IsOrdinal(_address.StreetName))
            {
                Result = _address;
                
                return;
            }

            // it's in the problem area in midvale
            const int midvale = 84047;
            if (((!string.IsNullOrEmpty(_city) && _city.ToUpperInvariant().Contains("MIDVALE")) || 
                (_address.Zip5.HasValue && _address.Zip5.Value == midvale)))
            {
                _address.PrefixDirection = Direction.West;
                Result = _address;
                
                return;
            }

            // update the slc avenues to have an east
            if (_address.AddressGrids.Select(x => x.Grid).Contains("SALT LAKE CITY"))
            {
                _address.PrefixDirection = Direction.East;
                Result = _address;

                return;
            }

            Result = _address;
        }

        private bool IsOrdinal(string streetname)
        {
            if (streetname.IsNullOrWhiteSpace())
            {
                return false;
            }

            streetname = streetname.Replace(" ", string.Empty).Trim();

            return _ordinal.IsMatch(streetname);
        }
    }
}