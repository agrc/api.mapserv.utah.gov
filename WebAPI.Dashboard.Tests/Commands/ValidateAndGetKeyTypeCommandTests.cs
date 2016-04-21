using NUnit.Framework;
using WebAPI.Common.Executors;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Dashboard.Areas.secure.Models.ViewModels;
using WebAPI.Dashboard.Commands.Key;

namespace WebAPI.Dashboard.Tests.Commands
{
    public class ValidateAndGetKeyTypeCommandTests
    {
        [Test]
        public void Empty_Parameters_Returns_None_with_Message()
        {
            var data = new ApiKeyData
                {
                    Ip = "",
                    UrlPattern = "",
                    AppStatus = ApiKey.ApplicationStatus.Production
                };

            var command = new ValidateAndGetKeyTypeCommand(data);
            var type = CommandExecutor.ExecuteCommand(command);

            Assert.That(type, Is.EqualTo(ApiKey.ApplicationType.None));
            Assert.That(command.ErrorMessage, Is.Not.Null);
        }

        [TestFixture]
        public class Browser
        {
            [TestFixture]
            public class Production
            {
                [TestCase("localhost/*")]
                [TestCase("localhost:1237")]
                [TestCase("localhost")]
                [TestCase("127.0.0.1")]
                [TestCase("127.0.0.1/app")]
                [TestCase("127.0.0.1/*")]
                [TestCase("machine-name")]
                [TestCase("agrc-sg1/*")]
                public void Bad_Url(string url)
                {
                    var data = new ApiKeyData
                        {
                            Ip = "",
                            UrlPattern = url,
                            AppStatus = ApiKey.ApplicationStatus.Production
                        };

                    var command = new ValidateAndGetKeyTypeCommand(data);
                    var type = CommandExecutor.ExecuteCommand(command);

                    Assert.That(type, Is.EqualTo(ApiKey.ApplicationType.None));
                    Assert.That(command.ErrorMessage, Is.Not.Null);
                }

                [TestCase("*.example.com/*")]
                [TestCase("http://atlas.utah.gov")]
                [TestCase("*.atlas.utah.gov/*")]
                [TestCase("*.mapserv.utah.gov/applicationname/*")]
                [TestCase("*.url.com/folder/appname/*")]
                [TestCase("*.nedds.health.utah.gov*")]
                [TestCase("api.utlegislators.com")]
                [TestCase("*168.177.222.22/app/*")]
                public void Good_Url(string url)
                {
                    var data = new ApiKeyData
                        {
                            Ip = "",
                            UrlPattern = url,
                            AppStatus = ApiKey.ApplicationStatus.Production
                        };

                    var command = new ValidateAndGetKeyTypeCommand(data);
                    var type = CommandExecutor.ExecuteCommand(command);

                    Assert.That(type, Is.EqualTo(ApiKey.ApplicationType.Browser));
                    Assert.That(command.ErrorMessage, Is.Null);
                }
            }

            [TestFixture]
            public class Development
            {
                [TestCase("localhost/*")]
                [TestCase("localhost")]
                [TestCase("127.0.0.1")]
                [TestCase("127.0.0.1/app")]
                [TestCase("127.0.0.1/*")]
                [TestCase("machine-name")]
                [TestCase("agrc-sg1/*")]
                [TestCase("*.example.com/*")]
                [TestCase("http://atlas.utah.gov")]
                [TestCase("*.atlas.utah.gov/*")]
                [TestCase("*.mapserv.utah.gov/applicationname/*")]
                [TestCase("*.url.com/folder/appname/*")]
                [TestCase("*.nedds.health.utah.gov*")]
                [TestCase("api.utlegislators.com")]
                [TestCase("*168.177.222.22/app/*")]
                public void Good_Url(string url)
                {
                    var data = new ApiKeyData
                    {
                        Ip = "",
                        UrlPattern = url,
                        AppStatus = ApiKey.ApplicationStatus.Development
                    };

                    var command = new ValidateAndGetKeyTypeCommand(data);
                    var type = CommandExecutor.ExecuteCommand(command);

                    Assert.That(type, Is.EqualTo(ApiKey.ApplicationType.Browser));
                    Assert.That(command.ErrorMessage, Is.Null);
                }
            }
        }
    }
}