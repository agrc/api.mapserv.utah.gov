using System.Text.RegularExpressions;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Areas.secure.Models.ViewModels;

namespace WebAPI.Dashboard.Commands.Key
{
    public class FormatKeyPatternCommand : Command<string>
    {
        private readonly Regex _hasHttp;
        private const string Https = "^htt(p|ps)://";

        public FormatKeyPatternCommand(ApiKey.ApplicationType type, ApiKeyData data)
        {
            Type = type;
            Data = data;
            _hasHttp = new Regex(Https, RegexOptions.IgnoreCase);
        }

        public ApiKey.ApplicationType Type { get; set; }
        public ApiKeyData Data { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Type: {1}, Data: {2}", "FormatKeyPatternCommand", Type, Data);
        }

        protected override void Execute()
        {
            if (Type != ApiKey.ApplicationType.Browser)
            {
                Result = null;
                return;
            }

            var pattern = _hasHttp.Replace(Data.UrlPattern, "").Replace(".",@"\.");

            var trimChars = new[] {'*'};
            if (pattern.StartsWith("*"))
            {
                if (pattern.StartsWith("*\\."))
                {
                    pattern = pattern.TrimStart(trimChars);
                    pattern = ".+" + pattern;
                }
                else
                {
                    pattern = pattern.TrimStart(trimChars);
                }
            }

            if (pattern.EndsWith("/*"))
            {
                pattern = pattern.TrimEnd(trimChars);
                pattern = pattern + ".*";
            }

            if (pattern.EndsWith("/"))
            {
                //pattern = pattern + "$";
            }

            pattern = Https + pattern;

            Result = pattern;
        }
    }
}