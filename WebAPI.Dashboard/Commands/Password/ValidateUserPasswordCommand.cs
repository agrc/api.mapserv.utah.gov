using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Common.Abstractions;
using WebAPI.Dashboard.Models.ViewModels.Users;

namespace WebAPI.Dashboard.Commands.Password
{
    public class ValidateUserPasswordCommand : Command<Task<bool>>
    {
        public ValidateUserPasswordCommand(string plainTextPassword, ValidateLoginCredentials credentials)
        {
            PlainTextPassword = plainTextPassword;
            Credentials = credentials;
        }

        public string PlainTextPassword { get; set; }
        public ValidateLoginCredentials Credentials { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, PlainTextPassword: {1}, Credentials: {2}", "ValidateUserPasswordCommand",
                                 PlainTextPassword, Credentials);
        }

        protected override void Execute()
        {
            Result = Task<bool>.Factory.StartNew(() =>
            {
                var enc = new UTF8Encoding();
                var keyGen = new Rfc2898DeriveBytes(PlainTextPassword + Credentials.Salt,
                                                    enc.GetBytes(
                                                        App.Pepper));
                var key = keyGen.GetBytes(32);
                var iv = keyGen.GetBytes(16);

                var cipher = new RijndaelManaged
                {
                    Key = key,
                    IV = iv
                };

                var plainText = enc.GetBytes(PlainTextPassword + Credentials.Salt);
                using (var encryptor = cipher.CreateEncryptor())
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor,
                                                         CryptoStreamMode.
                                                             Write))
                        {
                            cs.Write(plainText, 0, plainText.Length);
                            cs.FlushFinalBlock();

                            return Credentials.Password == enc.GetString(ms.ToArray());
                        }
                    }
                }
            });
        }
    }
}