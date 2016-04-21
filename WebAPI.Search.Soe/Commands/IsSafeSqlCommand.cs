using System.Text.RegularExpressions;
using Soe.Common.Infastructure.Commands;

namespace WebAPI.Search.Soe.Commands
{
    public class IsSafeSqlCommand : Command<bool>
    {
        public IsSafeSqlCommand(string text)
        {
            Text = text;
        }

        public IsSafeSqlCommand(string[] texts)
        {
            Texts = texts;
        }

        private string[] Texts { get; set; }

        private string Text { get; set; }

        protected override void Execute()
        {
            var badChars = new Regex(@";|--|/\*|\*/", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var validate = Text + string.Join("", Texts);

            Result = !badChars.IsMatch(validate);
        }

        public override string ToString()
        {
            return string.Format("{0}, Text: {1}", "IsSafeSqlCommand", Text);
        }
    }
}