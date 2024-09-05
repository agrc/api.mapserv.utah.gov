using EsriJson.Net;
using NetTopologySuite.Features;
using ugrc.api.Models.Constants;

namespace ugrc.api.Features.Converting;

public interface IConvertible<TInput> {
    object Convert(TInput input, ApiVersion? version);
    SerializableGraphic ToEsriJson(ApiVersion? version, int wkid);
    Feature ToGeoJson(ApiVersion? version, int wkid);
}

public interface IOutputConvertible {
    /// <summary>
    /// There are three output formats for the resulting street and zone geocoding. The **default** being empty.
    /// `esrijson` will parse into an `esri.Graphic` for mapping purposes and `geojson` will format as a
    /// [feature](https://tools.ietf.org/html/rfc7946#section-3.2). If this value is omitted, the default json will
    ///  be returned.
    /// </summary>
    [DefaultValue(JsonFormat.None)]
    public JsonFormat Format { get; set; }
}
