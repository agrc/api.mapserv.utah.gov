using System.Collections.Generic;

namespace AGRC.api.Comparers {
    public class StreetTypeAbbreviationComparer : IEqualityComparer<string> {
        public bool Equals(string x, string y) => x.Length == y.Length && string.Equals(x, y);

        public int GetHashCode(string obj) => obj.GetHashCode();
    }
}
