using System.Net;
using System.Net.Http;
using Moq;
using NUnit.Framework;
using Raven.Client.Documents;
using WebAPI.API.Handlers.Delegating;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Common.Providers;
using WebAPI.Common.Tests;
using WebAPI.Domain;

namespace WebAPI.API.Tests
{
    public class ApiKeyTests
    {
        [TestFixture]
        public class DesktopKeyTests : RavenEmbeddableTest
        {
            public override void SetUp()
            {
                base.SetUp();

                //arrange
                using (var s = DocumentStore.OpenSession())
                {
                    s.Store(new Account
                    {
                        FirstName = "test",
                        LastName = "account",
                        Confirmation = new EmailConfirmation("confirm")
                        {
                            Confirmed = true
                        }
                    }, "testaccount");

                    s.Store(new ApiKey("ip")
                    {
                        AccountId = "testaccount",
                        Key = "ipIsWrong",
                        CreatedAtTicks = 634940675825121039,
                        ApiKeyStatus = ApiKey.KeyStatus.Active,
                        Type = ApiKey.ApplicationType.Server,
                        AppStatus = ApiKey.ApplicationStatus.Production,
                        Pattern = "234.3.3.2",
                        RegexPattern = null,
                        IsMachineName = false,
                        Deleted = false
                    }, "testkey");

                    s.SaveChanges();
                }
            }

            [Test]
            public void FailsGracefullyWhenIncorrect()
            {
                var ipMoq = new Mock<IpProvider>();
                ipMoq.Setup(x => x.GetIp(It.IsAny<HttpRequestMessage>()))
                     .Returns("192.168.0.1");
                
                var authorizeRequestHandler = new AuthorizeRequestHandler
                {
                    InnerHandler = new TestHandler((r, c) => TestHandler.Return200()),
                    DocumentStore = InitDatabase(),
                    IpProvider = ipMoq.Object,
                    ApiKeyProvider = new ApiKeyProvider()
                };

                var client = new HttpClient(authorizeRequestHandler);
                var response = client.GetAsync("http://api.mapserv.utah.gov/api/v1/Geocode/326 east south temple/84111?apiKey=ipIsWrong").Result;

                var result = response.Content.ReadAsAsync<ResultContainer>().Result;

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.BadRequest));
                Assert.That(result.Message.StartsWith("Invalid API key."));
            }

            private IDocumentStore InitDatabase()
            {
                return DocumentStore;
            }
        }
    }
}