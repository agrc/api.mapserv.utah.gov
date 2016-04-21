using System;
using System.Security.Cryptography;
using WebAPI.Common.Abstractions;

namespace WebAPI.Dashboard.Commands.Password
{
    public class GenerateSaltCommand : Command<string>
    {
        protected override void Execute()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[25];

                randomNumberGenerator.GetBytes(bytes);

                Result = Convert.ToBase64String(bytes);
            }
        }

        public override string ToString()
        {
            return string.Format("GenerateSaltCommand");
        }
    }
}