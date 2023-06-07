namespace AGRC.api.Models;
/// <summary>
/// A coordinate pair collection representing a point
/// </summary>
public record Point(double X, double Y) {
    public string ToQuery() =>
        $$"""geometries={"geometryType":"esriGeometryPoint","geometries":[{"x":{{X}},"y":{{Y}}}]}""";
}
