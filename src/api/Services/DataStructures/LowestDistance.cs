namespace ugrc.api.Services;

public class DistanceComparer : IComparer<Map> {
    public int Compare(Map? x, Map? y) => (x?.Difference ?? int.MaxValue).CompareTo(y?.Difference ?? int.MaxValue);
}
public record Map(int Difference, string Zone);

public class LowestDistance : TopNList<Map> {
    public LowestDistance(int size) : base(size, new DistanceComparer()) {
    }
}
