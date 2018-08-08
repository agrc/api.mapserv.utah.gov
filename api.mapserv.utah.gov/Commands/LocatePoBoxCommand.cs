using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.RequestOptions;
using Serilog;

namespace api.mapserv.utah.gov.Commands
{
    public class LocatePoBoxCommand
    {
        private readonly ILookupCache _driveCache;
        private readonly ReprojectPointsCommand _repojectPoints;
        private GeocodingOptions _options;
        private GeocodeAddress _geocodedAddress;

        public LocatePoBoxCommand(ILookupCache driveCache, ReprojectPointsCommand repojectPoints)
        {
            _driveCache = driveCache;
            _repojectPoints = repojectPoints;
        }

        public void Initialize(GeocodeAddress geocodeAddress, GeocodingOptions options)
        {
            _options = options;
            _geocodedAddress = geocodeAddress;
        }

        public async Task<Candidate> Execute()
        {
            if (!_geocodedAddress.Zip5.HasValue)
            {
                Log.Debug("No zip code, can't be po box {address}", _geocodedAddress);

                return null;
            }

            if (_driveCache.PoBoxes is null)
            {
                Log.Warning("Po Box cache is empty!");

                return null;
            }

            if (!_driveCache.PoBoxes.ContainsKey(_geocodedAddress.Zip5.Value))
            {
                Log.Debug("{zip} is not in the po box cache", _geocodedAddress.Zip5.Value);

                return null;
            }

            Candidate candidate;
            var key = _geocodedAddress.Zip5.Value * 10000 + _geocodedAddress.PoBox;

            if (_driveCache.PoBoxZipCodesWithExclusions.Any(x => x == _geocodedAddress.Zip5) &&
                _driveCache.PoBoxExclusions.ContainsKey(key))
            {
                Log.Information("{Using Post Office Point Exclusion for {zip}", key);

                var exclusion = _driveCache.PoBoxExclusions[key];
                candidate = new Candidate
                {
                    Address = _geocodedAddress.StandardizedAddress,
                    Locator = "Post Office Point Exclusions",
                    Score = 100,
                    Location = new Point(exclusion.X, exclusion.Y),
                    AddressGrid = _geocodedAddress?.AddressGrids?.FirstOrDefault()?.Grid
                };
            }
            else if (_driveCache.PoBoxes.ContainsKey(_geocodedAddress.Zip5.Value))
            {
                Log.Information("Using post office point for {zip}", key);

                var result = _driveCache.PoBoxes[_geocodedAddress.Zip5.Value];
                candidate = new Candidate
                {
                    Address = _geocodedAddress.StandardizedAddress,
                    Locator = "Post Office Point",
                    Score = 100,
                    Location = new Point(result.X, result.Y),
                    AddressGrid = _geocodedAddress.AddressGrids.FirstOrDefault()?.Grid
                };
            }
            else
            {
                return null;
            }

            if (_options.SpatialReference == 26912)
            {
                return candidate;
            }

            _repojectPoints.Initialize(new ReprojectPointsCommand.PointProjectQueryArgs(26912, _options.SpatialReference,
                                                                                        new List<double>
                                                                                        {
                                                                                            candidate.Location.X,
                                                                                            candidate.Location.Y
                                                                                        }));

            var pointReprojectResponse = await _repojectPoints.Execute().ConfigureAwait(false);

            if (!pointReprojectResponse.IsSuccessful || !pointReprojectResponse.Geometries.Any())
            {
                Log.Fatal("Could not reproject point for {candidate}", candidate);

                return null;
            }

            var points = pointReprojectResponse.Geometries.FirstOrDefault();

            if (points != null)
            {
                candidate.Location = new Point(points.X, points.Y);
            }

            return candidate;
        }

        public override string ToString() => $"PoBoxCommand, GeocodeAddress: {_geocodedAddress}";
    }
}
