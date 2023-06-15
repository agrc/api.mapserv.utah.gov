using System.Text.Json.Serialization;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using Asp.Versioning;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace AGRC.api.Features.Converting;
public static class GeoJsonFeature {
    public class Computation(SingleGeocodeResponseContract container, ApiVersion? version) : IComputation<Feature> {
        public readonly SingleGeocodeResponseContract _result = container;
        public readonly int _version = version?.MajorVersion switch {
            1 => 1,
            2 => 2,
            _ => 1
        };
    }

    public class Handler(ILogger log) : IComputationHandler<Computation, Feature> {
        private readonly ILogger? _log = log?.ForContext<EsriGraphic>();
        private static string ToCamelCase(string data) => char.ToLowerInvariant(data[0]) + data[1..];

        public Task<Feature> Handle(Computation request, CancellationToken cancellationToken) {
            Geometry? geometry = null;
            var attributes = new Dictionary<string, object?>();
            var result = request._result;

            _log?.Debug("converting {result} to esri json for version {version}", result, request._version);

            if (result?.Location != null) {
                geometry = new Point(new Coordinate(result.Location.X, result.Location.Y));

                var properties = request._result?
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

                if (request._version == 1) {
                    foreach (var property in properties) {
                        var value = property.GetValue(request._result, null);

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
                        .ToDictionary(key => ToCamelCase(key.Name), value => value.GetValue(request._result, null))
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
                return Task.FromResult(new Feature());
            }

            var attributeTable = new AttributesTable(
                attributes.ToDictionary(x => x.Key, y => y.Value)
            );

            var feature = new Feature(geometry, attributeTable);

            return Task.FromResult(feature);
        }
    }
}
