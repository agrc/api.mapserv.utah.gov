using System.Text.Json.Serialization;
using AGRC.api.Extensions;
using AGRC.api.Features.Converting;
using AGRC.api.Models.Constants;
using EsriJson.Net;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace AGRC.api.Features.Milepost;
public record RouteMilepostResponseContract(string Source, Models.Point Location, string MatchRoute) : IConvertible<RouteMilepostRequestOptionsContract> {
    [JsonIgnore]
    public string InputRouteMilePost { get; set; } = string.Empty;

    public SerializableGraphic ToEsriJson(ApiVersion? version, int wkid) {
        EsriJsonObject? geometry = null;
        var attributes = new Dictionary<string, object?>();

        if (Location != null) {
            geometry = new EsriJson.Net.Geometry.Point(Location.X, Location.Y) {
                CRS = new Crs {
                    WellKnownId = wkid
                }
            };

            var properties = this?
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

            if ((version?.MajorVersion ?? 1) == 1) {
                attributes = properties.ToDictionary(key => key.Name, value => value.GetValue(this, null))
                ?? attributes;
            } else {
                attributes = properties
                    .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                    .ToDictionary(key => key.Name, value => value.GetValue(this, null))
                    ?? attributes;

                attributes.Remove("Location");

                if (attributes.Values.Any(x => x == null)) {
                    attributes = attributes.Where(x => x.Value != null)
                        .ToDictionary(x => x.Key, y => y.Value);
                }
            }
        }

        if (geometry == null && attributes.Count < 1) {
            return new SerializableGraphic(new Graphic(null, null));
        }

        var graphic =
            new Graphic(geometry, attributes
                .ToDictionary(x => x.Key, y => y.Value));

        return new SerializableGraphic(graphic);
    }
    public Feature ToGeoJson(ApiVersion? version, int wkid) {
        Geometry? geometry = null;
        var attributes = new Dictionary<string, object?>();

        if (Location != null) {
            geometry = new Point(new Coordinate(Location.X, Location.Y));

            var properties = this?
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public) ?? Array.Empty<PropertyInfo>();

            if ((version?.MajorVersion ?? 1) == 1) {
                foreach (var property in properties) {
                    var value = property.GetValue(this, null);
                    attributes.Add(property.Name, value);
                }
            } else {
                attributes = properties
                    .Where(prop => !Attribute.IsDefined(prop, typeof(JsonIgnoreAttribute)))
                    .ToDictionary(key => key.Name.ToCamelCase(), value => value.GetValue(this, null))
                    ?? attributes;

                attributes.Remove("location");

                if (attributes.Any()) {
                    attributes.Add("srid", wkid);
                }

                if (attributes.Values.Any(x => x == null)) {
                    attributes = attributes.Where(x => x.Value != null)
                        .ToDictionary(x => x.Key, y => y.Value);
                }
            }
        }

        if (geometry == null && attributes.Count < 1) {
            return new Feature();
        }

        var attributeTable = new AttributesTable(
            attributes.ToDictionary(x => x.Key, y => y.Value)
        );

        return new Feature(geometry, attributeTable);
    }
    public object Convert(RouteMilepostRequestOptionsContract options, ApiVersion? version)
    => options.Format switch {
        JsonFormat.EsriJson => ToEsriJson(version, options.SpatialReference),
        JsonFormat.GeoJson => ToGeoJson(version, options.SpatialReference),
        JsonFormat.None => this,
        _ => throw new NotImplementedException(),
    };
}
