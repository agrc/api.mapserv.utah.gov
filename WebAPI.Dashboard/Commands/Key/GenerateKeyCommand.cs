using System;
using System.Globalization;
using WebAPI.Common.Abstractions;

namespace WebAPI.Dashboard.Commands.Key
{
    public class GenerateKeyCommand : Command<string>
    {
        public override string ToString()
        {
            return "GenerateKeyCommand";
        }

        protected override void Execute()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var guid = Guid.NewGuid().ToString();
            var alpha = guid.Substring(0, guid.IndexOf("-", StringComparison.Ordinal)).ToUpper();

            var key = string.Format("AGRC-{0}{1}", alpha,
                                    rand.Next(100000, 999999).ToString(CultureInfo.InvariantCulture));
            Result = key;
        }
    }
}