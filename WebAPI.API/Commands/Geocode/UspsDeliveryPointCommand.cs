using System.Collections.Generic;
using System.Linq;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Commands.Spatial;
using WebAPI.Common.Executors;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;
using WebAPI.Domain.Linkers;

namespace WebAPI.API.Commands.Geocode
{
    public class UspsDeliveryPointCommand : Command<IEnumerable<Candidate>>
    {
        private readonly GeocodeOptions _options;

        public UspsDeliveryPointCommand(GeocodeAddress geocodeAddress, GeocodeOptions options)
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

            List<GridLinkable> items;
            App.UspsDeliveryPoints.TryGetValue(GeocodedAddress.Zip5.Value.ToString(), out items);

            if (items == null || !items.Any())
            {
                Result = null;
                return;
            }

            var deliveryPoint = items.FirstOrDefault() as UspsDeliveryPointLink;

            if (deliveryPoint == null)
            {
                Result = null;
                return;
            }

            var result = new Candidate
                {
                    Address = deliveryPoint.MatchAddress,
                    AddressGrid = deliveryPoint.Grid,
                    Locator = "USPS Delivery Points",
                    Score = 100,
                    Location = new Location(deliveryPoint.X, deliveryPoint.Y)
                };

            if (_options.WkId != 26912)
            {
                var reprojectPointCommand =
                    new ReprojectPointsCommand(new ReprojectPointsCommand.PointProjectQueryArgs(26912, _options.WkId,
                                                                                                new List<double>
                                                                                                    {
                                                                                                        deliveryPoint.X,
                                                                                                        deliveryPoint.Y
                                                                                                    }));

                var pointReprojectResponse = CommandExecutor.ExecuteCommand(reprojectPointCommand);

                if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
                    return;

                var points = pointReprojectResponse.Geometries.FirstOrDefault();

                result.Location = new Location(points.X, points.Y);
            }

            Result = new[] {result};
        }

        public override string ToString()
        {
            return string.Format("{0}, Options: {1}, GeocodedAddress: {2}", "UspsDeliveryPointCommand", _options,
                                 GeocodedAddress);
        }
    }
}