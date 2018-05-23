using System.Text.RegularExpressions;

namespace api.mapserv.utah.gov.Cache
{
    public interface IRegexCache
    {
        Regex Get(string key);
    }
}