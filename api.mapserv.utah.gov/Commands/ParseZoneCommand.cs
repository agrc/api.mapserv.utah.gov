﻿using System.Linq;
using System.Text.RegularExpressions;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Services;
using Serilog;

namespace api.mapserv.utah.gov.Commands
{
  public class ParseZoneCommand : Command<GeocodeAddress>
    {
        private readonly IRegexCache _regex;
        private readonly ILookupCache _driveCache;

        public ParseZoneCommand(IRegexCache regex, ILookupCache driveCache)
        {
            _regex = regex;
            _driveCache = driveCache;
        }
        public void Initialize(string inputZone, GeocodeAddress addressModel)
        {
            InputZone = inputZone;
            AddressModel = addressModel;
        }

        public string InputZone { get; set; }

        public GeocodeAddress AddressModel { get; set; }

        public override string ToString() => $"ParseZoneCommand, InputZone: {InputZone}, AddressModel: {AddressModel}";

        protected override void Execute()
        {
            Log.Debug("Parsing {zone}", InputZone);

            InputZone = InputZone.Replace("-", "");
            var zipPlusFour = _regex.Get("zipPlusFour").Match(InputZone);

            if (zipPlusFour.Success)
            {

                if (zipPlusFour.Groups[1].Success)
                {
                    var zip5 = zipPlusFour.Groups[1].Value;
                    Log.Debug("Zone has a zip code of {zip}", zip5);

                    AddressModel.Zip5 = int.Parse(zip5);

                    var getAddressSystemFromZipCodeCommand = new GetAddressSystemFromZipCodeCommand(_driveCache);
                    getAddressSystemFromZipCodeCommand.Initialize(AddressModel.Zip5);

                    AddressModel.AddressGrids = CommandExecutor.ExecuteCommand(getAddressSystemFromZipCodeCommand);
                }

                if (zipPlusFour.Groups[2].Success)
                {
                    var zip4 = zipPlusFour.Groups[2].Value;

                    Log.Debug("Zone has a zip + 4 {zip}", zip4);

                    AddressModel.Zip4 = int.Parse(zip4);
                }

                Result = CommandExecutor.ExecuteCommand(new DoubleAvenuesExceptionCommand(AddressModel, ""));

                return;
            }

            var cityName = _regex.Get("cityName").Match(InputZone);

            if (cityName.Success)
            {
                Log.Debug("Zone is a place {place}", cityName.Value);

                var cityKey = cityName.Value.ToLower();
                cityKey = cityKey.Replace(".", "");
                cityKey = Regex.Replace(cityKey, @"\s+", " ");

                cityKey = _regex.Get("cityTownCruft").Replace(cityKey, "").Trim();

                var getAddressSystemFromCityCommand = new GetAddressSystemFromCityCommand(_driveCache);
                getAddressSystemFromCityCommand.Initialize(cityKey);

                AddressModel.AddressGrids = CommandExecutor.ExecuteCommand(getAddressSystemFromCityCommand);

                Result = CommandExecutor.ExecuteCommand(new DoubleAvenuesExceptionCommand(AddressModel, cityKey));

                return;
            }

            if (AddressModel.AddressGrids == null)
            {
                Log.Warning("No address grid found for {zone}", InputZone);

                AddressModel.AddressGrids = Enumerable.Empty<GridLinkable>().ToList();
            }

            Result = AddressModel;
        }
    }
}