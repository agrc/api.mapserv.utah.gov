using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.RequestOptions;
using api.mapserv.utah.gov.Services;

namespace api.mapserv.utah.gov.Commands
{
    public class UspsDeliveryPointCommand
    {
        private readonly ILookupCache _driveCache;
        private readonly ReprojectPointsCommand _reproject;
        private GeocodingOptions _options;

        public UspsDeliveryPointCommand(ILookupCache drivecCache, ReprojectPointsCommand reproject)
        {
            _driveCache = drivecCache;
            _reproject = reproject;
        }

        private GeocodeAddress GeocodedAddress { get; set; }

        public void Initialize(GeocodeAddress geocodeAddress, GeocodingOptions options)
        {
            _options = options;
            GeocodedAddress = geocodeAddress;
        }

        public async Task<Candidate> Execute()
        {
            if (!GeocodedAddress.Zip5.HasValue)
            {
                return null;
            }

            _driveCache.UspsDeliveryPoints.TryGetValue(GeocodedAddress.Zip5.Value.ToString(), out List<GridLinkable> items);

            if (items == null || !items.Any())
            {
                return null;
            }

            if (!(items.FirstOrDefault() is UspsDeliveryPointLink deliveryPoint))
            {
                return null;
            }

            var result = new Candidate
            {
                Address = deliveryPoint.MatchAddress,
                AddressGrid = deliveryPoint.Grid,
                Locator = "USPS Delivery Points",
                Score = 100,
                Location = new Point(deliveryPoint.X, deliveryPoint.Y)
            };

            if (_options.SpatialReference == 26912)
            {
                return result;
            }

            _reproject.Initialize(new ReprojectPointsCommand.PointProjectQueryArgs(26912, _options.SpatialReference,
                                                                                   new List<double>
                                                                                   {
                                                                                       deliveryPoint.X,
                                                                                       deliveryPoint.Y
                                                                                   }));

            var pointReprojectResponse = await _reproject.Execute();

            if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
            {
                return null;
            }

            var points = pointReprojectResponse.Geometries.FirstOrDefault();

            if (points != null)
            {
                result.Location = new Point(points.X, points.Y);
            }

            return result;
        }

        public override string ToString() => $"UspsDeliveryPointCommand, Options: {_options}, GeocodedAddress: {GeocodedAddress}";
    }
}
