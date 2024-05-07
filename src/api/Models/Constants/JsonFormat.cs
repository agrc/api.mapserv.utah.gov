namespace ugrc.api.Models.Constants;
/// <summary>
///     The shape of the output model. esri json will easily parse into an esri.Graphic for display on a map and
///     geojson will easily parse into a feature for use in many open source projects. If this value is omitted, the
///     default json will be returned.
/// </summary>
public enum JsonFormat {
    EsriJson,
    GeoJson,
    None
}
