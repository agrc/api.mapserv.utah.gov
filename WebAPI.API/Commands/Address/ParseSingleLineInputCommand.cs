using System.Linq;
using System.Text.RegularExpressions;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Address
{
    public class ParseSingleLineInputCommand : Command<ParseSingleLineInputCommand.Address>
    {
        private string _inputAddress;

        public ParseSingleLineInputCommand(string inputAddress)
        {
            _inputAddress = inputAddress;
        }

        public ParseSingleLineInputCommand()
        {
        }

        public void SetInput(string input)
        {
            _inputAddress = input;
        }

        public override string ToString()
        {
            return string.Format("{0}, InputAddress: {1}", "ParseSingleLineInputCommand", _inputAddress);
        }

        protected override void Execute()
        {
            var zone = "";
            var singleLineAddress = _inputAddress.Replace(".", "").Replace(",", "").Trim();

            var stripUtah = new Regex("(?:ut)(?:ah)?$", RegexOptions.IgnoreCase);
            singleLineAddress = stripUtah.Replace(singleLineAddress, "").Trim();

            var zipPlusFour = new Regex(@"\s(\d{5})-?(\d{4})?$");
            var match = zipPlusFour.Match(singleLineAddress);

            if (match.Success)
            {
                if (match.Groups[1].Success)
                {
                    zone = match.Groups[1].Value;
                    singleLineAddress = zipPlusFour.Replace(singleLineAddress, "").Trim();
                }
            }

            var zones = App.PlaceGridLookup.Where(x => singleLineAddress.ToLower().Contains(x.Key.ToLower())).ToList();

            if (zones.Count == 1)
            {
                zone = zones.First().Key;
            }
            else if(zones.Count > 1)
            {
                // we want to get rid of the longest one
                // sometimes the longest one might be the wrong one
                // like if salt lake city was a part of the address and logan was the zone
                zone = zones.OrderByDescending(x => x.Key.Length).First().Key;
            }
            
            // strip ut again if it wasn't at the end to begin with
            var street = stripUtah.Replace(singleLineAddress, "").Trim();
            street = new Regex("\\s" + zone + "$", RegexOptions.IgnoreCase).Replace(street, "").Trim();

            Result = new Address(street, zone);
        }

        public class Address
        {
            public Address(string street, string zone)
            {
                Street = street;
                Zone = zone;
            }

            public string Street { get; set; }
            public string Zone { get; set; }
        }
    }
}