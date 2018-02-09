using System.Text.RegularExpressions;
using WebAPI.API.Commands.Geocode.Flags;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;

namespace WebAPI.API.Commands.Address
{
    public class ParseZoneCommand : Command<GeocodeAddress>
    {
        public ParseZoneCommand(string inputZone, GeocodeAddress addressModel)
        {
            InputZone = inputZone;
            AddressModel = addressModel;
        }

        public string InputZone { get; set; }

        public GeocodeAddress AddressModel { get; set; }

        public override string ToString()
        {
            return $"ParseZoneCommand, InputZone: {InputZone}, AddressModel: {AddressModel}";
        }

        protected override void Execute()
        {
            var zipPlusFour = App.RegularExpressions["zipPlusFour"].Match(InputZone);

            if (zipPlusFour.Success)
            {
                if (zipPlusFour.Groups[1].Success)
                {
                    var zip5 = zipPlusFour.Groups[1].Value;
                    AddressModel.Zip5 = int.Parse(zip5);

                    AddressModel.AddressGrids = CommandExecutor.ExecuteCommand(
                        new GetAddressSystemFromZipCodeCommand(AddressModel.Zip5));
                }

                if (zipPlusFour.Groups[2].Success)
                {
                    var zip4 = zipPlusFour.Groups[2].Value;
                    AddressModel.Zip4 = int.Parse(zip4);
                }

                Result = CommandExecutor.ExecuteCommand(new DoubleAvenuesExceptionCommand(AddressModel, ""));
                return;
            }

            var cityName = App.RegularExpressions["cityName"].Match(InputZone);

            if (cityName.Success)
            {
                var cityKey = cityName.Value.ToLower();
                cityKey = cityKey.Replace(".", "");
                cityKey = Regex.Replace(cityKey, @"\s+", " ");

                cityKey = App.RegularExpressions["cityTownCruft"].Replace(cityKey, "").Trim();

                AddressModel.AddressGrids = CommandExecutor.ExecuteCommand(
                    new GetAddressSystemFromCityCommand(cityKey));

                Result = CommandExecutor.ExecuteCommand(new DoubleAvenuesExceptionCommand(AddressModel, cityKey));
                return;
            }

            Result = AddressModel;
        }
    }
}