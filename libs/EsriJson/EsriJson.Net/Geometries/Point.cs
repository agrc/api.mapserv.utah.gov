namespace EsriJson.Net.Geometry;
public class Point : EsriJsonObject {
    public Point() {

    }

    public Point(double x, double y) {
        X = x;
        Y = y;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public bool Equals(Point obj) => obj != null && obj.X == X && obj.Y == Y;

    public override string Type => "point";
}
