using System;
using System.Globalization;
using System.Text;
using WebAPI.Common.Abstractions;

namespace WebAPI.Dashboard.Commands.Password
{
    public class GeneratePasswordCommand : Command<string>
    {
        public GeneratePasswordCommand()
            : this(10) {}

        public GeneratePasswordCommand(int length)
        {
            Length = length;
        }

        public int Length { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Length: {1}", "GeneratePasswordCommand", Length);
        }

        protected override void Execute()
        {
            var output = String.Empty;

            var rnd = new Random();
            const int max = 4;

            for (var index = 0; index < Length; index++)
            {
                var number = rnd.Next(1, max + 1);

                switch (number)
                {
                    case 1:
                        // uppercase english alphabet
                        var uppercased = (Byte) rnd.Next(65, 91);
                        output += Encoding.ASCII.GetString(new[]
                        {
                            uppercased
                        });
                        break;
                    case 2:
                        // lowercase english alphabet
                        var lowercased = (Byte) rnd.Next(97, 123);
                        output += Encoding.ASCII.GetString(new[]
                        {
                            lowercased
                        });
                        break;
                    case 3:
                        // the numbers, 0 to 9
                        output += rnd.Next(0, 10).ToString(CultureInfo.InvariantCulture);
                        break;
                    case 4:
                        // the symbols, 33 to 47
                        var symbol = (Byte) rnd.Next(33, 47);
                        output += Encoding.ASCII.GetString(new[]
                        {
                            symbol
                        });
                        break;
                    default:
                        throw new ApplicationException("Ups");
                }
            }

            Result = output;
        }
    }
}