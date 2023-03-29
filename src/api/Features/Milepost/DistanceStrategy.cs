using AGRC.api.Models;

namespace AGRC.api.Features.Milepost;
public interface IDistanceStrategy {
    double Calculate(Point from, Point to, int fractionalDigits = -1);
}

public class PythagoreanDistance : IDistanceStrategy {
    public double Calculate(Point? from, Point? to, int fractionDigits) {
        if (from is null || to is null) {
            return double.NaN;
        }

        var dx = from.X - to.X;
        var dy = from.Y - to.Y;

        var d = Math.Pow(dx, 2) + Math.Pow(dy, 2);
        var distance = Math.Sqrt(d);

        if (fractionDigits != -1) {
            distance = Math.Round(distance, fractionDigits);
        }

        return distance;
    }
}
