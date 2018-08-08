using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.RequestOptions;
using Serilog;

namespace api.mapserv.utah.gov.Commands
{
    public class UspsDeliveryPointCommand
    {
        private readonly ILookupCache _driveCache;
        private readonly ReprojectPointsCommand _reproject;
        private GeocodingOptions _options;

        public UspsDeliveryPointCommand(ILookupCache driveCache, ReprojectPointsCommand reproject)
        {
            _driveCache = driveCache;
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
            Log.Verbose("Testing for delivery points");

            if (!GeocodedAddress.Zip5.HasValue)
            {
                Log.Debug("No delivery point for {address} because of no zip5", GeocodedAddress);

                return null;
            }

            _driveCache.UspsDeliveryPoints.TryGetValue(GeocodedAddress.Zip5.Value.ToString(), out List<GridLinkable> items);

            if (items == null || !items.Any())
            {
                Log.Debug("No delivery point for {zip} in cache", GeocodedAddress.Zip5.Value);

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

            Log.Information("Found delivery point for {address}", GeocodedAddress);

            if (_options.SpatialReference == 26912)
            {
                return result;
            }

            Log.Debug("Reprojecting delivery point to {wkid}", _options.SpatialReference);

            _reproject.Initialize(new ReprojectPointsCommand.PointProjectQueryArgs(26912, _options.SpatialReference,
                                                                                   new List<double>
                                                                                   {
                                                                                       deliveryPoint.X,
                                                                                       deliveryPoint.Y
                                                                                   }));

            var pointReprojectResponse = await _reproject.Execute();

            if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
            {
                Log.Fatal("Could not reproject point for {address}", GeocodedAddress);

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
