using System.Text.RegularExpressions;

namespace AGRC.api.Cache {
    public interface IRegexCache {
        Regex Get(string key);
    }
}
