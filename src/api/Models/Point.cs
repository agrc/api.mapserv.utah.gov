namespace AGRC.api.Models;
/// <summary>
/// A coordinate pair collection representing a point
/// </summary>
public record Point(double X, double Y) {
    public string ToQuery() =>
        $$"""geometries={"geometryType":"esriGeometryPoint","geometries":[{"x":{{X}},"y":{{Y}}}]}""";
}

public record SpatialReference(int Wkid, int? LatestWkid) {
    public int Srid => LatestWkid ?? Wkid;
};
public record PointWithSpatialReference(double X, double Y, SpatialReference SpatialReference) : Point(X, Y) {
    public string ToEsriJson() =>
        $$$"""{"x":{{{X}}},"y":{{{Y}}},"spatialReference":{"wkid":{{{SpatialReference.Srid}}}}}""";

    public string ToPostGis() => SpatialReference.Srid switch {
        26912 => $"st_pointfromtext('POINT({X} {Y})',{SpatialReference.Srid})",
        _ => $"st_transform(st_pointfromtext('POINT({X} {Y})',{SpatialReference.Srid}),26912)"
    };
};
