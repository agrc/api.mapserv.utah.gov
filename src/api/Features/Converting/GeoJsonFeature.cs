using System.Text.Json.Serialization;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using Asp.Versioning;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace AGRC.api.Features.Converting;
public class GeoJsonFeature {
    public class Computation : IComputation<ApiResponseContract<Feature>> {
        internal readonly ApiResponseContract<SingleGeocodeResponseContract> Container;
        internal readonly int Version;

        public Computation(ApiResponseContract<SingleGeocodeResponseContract> container, ApiVersion? version) {
            Container = container;
            Version = version?.MajorVersion switch {
                1 => 1,
                2 => 2,
                _ => 1
            };
        }
    }

    public class Handler : IComputationHandler<Computation, ApiResponseContract<Feature>> {
        private readonly ILogger? _log;
        private static string ToCamelCase(string data) => char.ToLowerInvariant(data[0]) + data[1..];
        public Handler(ILogger log) {
            _log = log?.ForContext<EsriGraphic>();
        }
        public Task<ApiResponseContract<Feature>> Handle(Computation request, CancellationToken cancellationToken) {
            Geometry? geometry = null;
            var attributes = new Dictionary<string, object?>();
            var message = request.Container.Message;
            var status = request.Container.Status;
            var result = request.Container.Result;

            _log?.Debug("converting {result} to esri json for version {version}", result, request.Version);

            if (result?.Location != null) {
                geometry = new Point(new Coordinate(result.Location.X, result.Location.Y));

                var properties = request.Container.Result?
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

                if (request.Version == 1) {
                    foreach (var property in properties) {
                        var value = property.GetValue(request.Container.Result, null);

                        if (property.Name.Equals("standardizedAddress", StringComparison.OrdinalIgnoreCase) && value is null) {
                            continue;
                        }

                        if (property.Name.Equals("scoreDifference", StringComparison.OrdinalIgnoreCase) && value is null) {
                            value = -1;
                        }

                        if (property.Name.Equals("candidates", StringComparison.OrdinalIgnoreCase) && value is null) {
                            value = Array.Empty<object>();
                        }

                        attributes.Add(property.Name, value);
                    }
                } else {
                    attributes = properties
                        .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                        .ToDictionary(key => ToCamelCase(key.Name), value => value.GetValue(request.Container.Result, null))
                        ?? attributes;

                    attributes.Remove("location");

                    if (attributes.Any()) {
                        attributes.Add("srid", result.Wkid);
                    }

                    if (attributes.Values.Any(x => x == null)) {
                        attributes = attributes.Where(x => x.Value != null)
                            .ToDictionary(x => x.Key, y => y.Value);
                    }
                }
            }

            if (geometry == null && attributes.Count < 1) {
                return Task.FromResult(new ApiResponseContract<Feature> {
                    Status = status,
                    Message = message
                });
            }

            var attributeTable = new AttributesTable(
                attributes.ToDictionary(x => x.Key, y => y.Value)
            );

            var feature = new Feature(geometry, attributeTable);

            var responseContainer = new ApiResponseContract<Feature> {
                Result = feature,
                Status = status,
                Message = message
            };

            return Task.FromResult(responseContainer);
        }
    }
}
