using System.Text.RegularExpressions;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Search
{
    public class IsSafeSqlCommand : Command<bool>
    {
        public string Text { get; set; }

        public IsSafeSqlCommand(string text)
        {
            Text = text;
        }

        protected override void Execute()
        {
            var badChars = new Regex(@";|--|/\*|\*/", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            Result = badChars.IsMatch(Text);
        }

        public override string ToString()
        {
            return string.Format("{0}, Text: {1}", "IsSafeSqlCommand", Text);
        }
    }
}