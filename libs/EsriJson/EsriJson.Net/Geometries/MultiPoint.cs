using System;

namespace EsriJson.Net.Geometry;
public class MultiPoint : EsriJsonObject {
    public MultiPoint() { }

    public MultiPoint(Point[] points) {
        Points = points;
    }

    public Point[] Points { get; set; } = Array.Empty<Point>();

    public override string Type => "multiPoint";
}
