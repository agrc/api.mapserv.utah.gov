namespace ugrc.api.Comparers;
public class StreetTypeAbbreviationComparer : IEqualityComparer<string> {
    public bool Equals(string? x, string? y) {
        if (x is null && y is null) {
            return true;
        }

        if (y is null) {
            return false;
        }

        if (x is null) {
            return false;
        }

        return x.Length == y.Length && string.Equals(x, y);
    }

    public int GetHashCode(string obj) => obj.GetHashCode();
}
