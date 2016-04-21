using System.Collections.Generic;
using System.Linq;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Commands.Spatial;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Commands.Geocode
{
    public class PoBoxCommand : Command<IEnumerable<Candidate>>
    {
        private readonly GeocodeOptions _options;

        public PoBoxCommand(GeocodeAddress geocodeAddress, GeocodeOptions options)
        {
            _options = options;
            GeocodedAddress = geocodeAddress;
        }

        private GeocodeAddress GeocodedAddress { get; set; }

        protected override void Execute()
        {
            if (!GeocodedAddress.Zip5.HasValue)
            {
                Result = null;
                return;
            }

            if (!App.PoBoxLookup.ContainsKey(GeocodedAddress.Zip5.Value))
            {
                Result = null;
                return;
            }

            Candidate candidate;
            var key = GeocodedAddress.Zip5.Value*10000 + GeocodedAddress.PoBox;
            if (App.PoBoxZipCodesWithExclusions.Any(x => x == GeocodedAddress.Zip5) &&
                App.PoBoxExclusions.ContainsKey(key))
            {
                var exclusion = App.PoBoxExclusions[key];
                candidate = new Candidate
                {
                    Address = GeocodedAddress.StandardizedAddress,
                    Locator = "Post Office Point Exclusions",
                    Score = 100,
                    Location = new Location(exclusion.X, exclusion.Y),
                    AddressGrid = GeocodedAddress.AddressGrids.FirstOrDefault().Grid
                };
            }
            else
            {
                var result = App.PoBoxLookup[GeocodedAddress.Zip5.Value];
                candidate = new Candidate
                {
                    Address = GeocodedAddress.StandardizedAddress,
                    Locator = "Post Office Point",
                    Score = 100,
                    Location = new Location(result.X, result.Y),
                    AddressGrid = GeocodedAddress.AddressGrids.FirstOrDefault().Grid
                };
            }

            if (_options.WkId != 26912)
            {
                var reprojectPointCommand =
                    new ReprojectPointsCommand(new ReprojectPointsCommand.PointProjectQueryArgs(26912, _options.WkId,
                                                                                                new List<double>
                                                                                                    {
                                                                                                        candidate.Location.X,
                                                                                                        candidate.Location.Y
                                                                                                    }));

                var pointReprojectResponse = CommandExecutor.ExecuteCommand(reprojectPointCommand);

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
                    return;

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                candidate.Location = new Location(points.X, points.Y);
            }

            Result = new[] { candidate };
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, GeocodeAddress: {1}", "PoBoxCommand", GeocodedAddress);
        }
    }
}