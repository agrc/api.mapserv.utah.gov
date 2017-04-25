using System.Net.Http;
using System.Net.Http.Formatting;
using Moq;
using NUnit.Framework;
using WebAPI.API.Handlers.Delegating;
using WebAPI.API.Models;
using WebAPI.Common.Executors;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;
using WebAPI.Common.Providers;
using WebAPI.Common.Tests;
using WebAPI.Dashboard.Areas.secure.Models.ViewModels;
using WebAPI.Dashboard.Commands.Key;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;

namespace WebAPI.API.Tests.Handlers
{
    public class AuthorizationHandlerTests
    {
        [TestFixture]
        public class Browser : RavenEmbeddableTest
        {
            public override void SetUp()
            {
                base.SetUp();

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

                    s.SaveChanges();
                }
            }

            [TestCase("www.example.com", "http://www.example.com/", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/request/test.html", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]
            
            [TestCase("www.example.com", "http://www.badexample.com/", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/request/test.html", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/request/test/index.html?query=yes", ExpectedResult = 400)]

            [TestCase("www.example.com/*", "http://www.example.com/", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/reqes/test.html", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com/*", "http://www.badexample.com/", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/request/test.html", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/request/test/index.html?query=yes", ExpectedResult = 400)]

            [TestCase("www.example.com/", "http://www.example.com", ExpectedResult = 200)]
            [TestCase("www.example.com/", "http://www.example.com/index.html", ExpectedResult = 200)]

            [TestCase("example.com/*", "http://example.com/index.html", ExpectedResult = 200)]
            [TestCase("example.com/*", "http://example.com/request/index.html", ExpectedResult = 200)]

            [TestCase("example.com/*", "http://bad.example.com/index.html", ExpectedResult = 400)]
            [TestCase("example.com/*", "http://bad.example.com/request/index.html", ExpectedResult = 400)]

            [TestCase("*.example.com", "http://any.example.com/", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/index.html", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/request/test.html", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com/test", "http://www.example.com/test/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/test", "http://www.example.com/test", ExpectedResult = 200)]

            [TestCase("www.example.com/test", "http://www.example.com/bad", ExpectedResult = 400)]
            [TestCase("www.example.com/test", "http://www.example.com/bad/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/test", "http://bad.example.com/test/index.html", ExpectedResult = 400)]

            [TestCase("www.example.com/test/*", "http://www.example.com/test/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/test/*", "http://www.example.com/test/test2/index.html", ExpectedResult = 200)]

            [TestCase("www.example.com/test/*", "http://bad.example.com/test/test/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/test/*", "http://www.example.com/bad/test2/index.htm", ExpectedResult = 400)]

            [TestCase("*.nedds.health.utah.gov*", "http://www.nedds.health.utah.gov", ExpectedResult = 200)]
            [TestCase("api.utlegislators.com", "http://api.utlegislators.com", ExpectedResult = 200)]
            [TestCase("*168.177.222.22/app/*", "http://168.177.222.22/app/whatever", ExpectedResult = 200)]
            public int ProductionKey_IsValid(string pattern, string url)
            {
                //arrange
                const ApiKey.ApplicationType applicationType = ApiKey.ApplicationType.Browser;
                var data = new ApiKeyData
                    {
                        AppStatus = ApiKey.ApplicationStatus.Production,
                        UrlPattern = pattern
                    };

                using (var s = DocumentStore.OpenSession())
                {
                    s.Store(new ApiKey("key")
                        {
                            AccountId = "testaccount",
                            CreatedAtTicks = 634940675825121039,
                            ApiKeyStatus = ApiKey.KeyStatus.Active,
                            Type = applicationType,
                            AppStatus = ApiKey.ApplicationStatus.Production,
                            RegexPattern =
                                CommandExecutor.ExecuteCommand(new FormatKeyPatternCommand(applicationType, data)),
                            Pattern = null,
                            IsMachineName = false,
                            Deleted = false
                        }, "testkey");

                    s.SaveChanges();
                }

                var content =
                    new ObjectContent<ResultContainer<GeocodeAddressResult>>(new ResultContainer<GeocodeAddressResult>
                        {
                            Result = new GeocodeAddressResult
                                {
                                    InputAddress = "tESTING",
                                    Score = 100
                                }
                        }, new JsonMediaTypeFormatter());

                content.Headers.Add("X-Type", typeof (ResultContainer<GeocodeAddressResult>).ToString());

                var contentMoq = new Mock<HttpContentProvider>();
                contentMoq.Setup(x => x.GetResponseContent(It.IsAny<HttpResponseMessage>()))
                          .Returns(content);

                var handler = new AuthorizeRequestHandler
                    {
                        DocumentStore = DocumentStore,
                        InnerHandler = new TestHandler((r, c) => TestHandler.Return200()),
                        ApiKeyProvider = new ApiKeyProvider()
                    };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("Referrer", new[] {url});
                client.DefaultRequestHeaders.Add("Referer", new[] { url });

                var result =
                    client.GetAsync("http://webapi/v1/Geocode/326 east south temple/84111?apiKey=key")
                          .Result;

                return (int) result.StatusCode;
            }

            [TestCase("www.example.com/test/*", "http://bad.example.com/test/test/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/test/*", "http://www.example.com/bad/test2/index.htm", ExpectedResult = 400)]
            [TestCase("machine-name", "http://machine-name/index.html", ExpectedResult = 200)]
            [TestCase("machine-name/*", "http://machine-name/beta/index.htm", ExpectedResult = 200)]
            public int DevKey_IsValid(string pattern, string url)
            {
                //arrange
                const ApiKey.ApplicationType applicationType = ApiKey.ApplicationType.Browser;
                var data = new ApiKeyData
                {
                    AppStatus = ApiKey.ApplicationStatus.Development,
                    UrlPattern = pattern
                };

                using (var s = DocumentStore.OpenSession())
                {
                    s.Store(new ApiKey("key")
                    {
                        AccountId = "testaccount",
                        CreatedAtTicks = 634940675825121039,
                        ApiKeyStatus = ApiKey.KeyStatus.Active,
                        Type = applicationType,
                        AppStatus = ApiKey.ApplicationStatus.Development,
                        RegexPattern =
                            CommandExecutor.ExecuteCommand(new FormatKeyPatternCommand(applicationType, data)),
                        Pattern = null,
                        IsMachineName = false,
                        Deleted = false
                    }, "testkey");

                    s.SaveChanges();
                }

                var content =
                    new ObjectContent<ResultContainer<GeocodeAddressResult>>(new ResultContainer<GeocodeAddressResult>
                    {
                        Result = new GeocodeAddressResult
                        {
                            InputAddress = "tESTING",
                            Score = 100
                        }
                    }, new JsonMediaTypeFormatter());

                content.Headers.Add("X-Type", typeof(ResultContainer<GeocodeAddressResult>).ToString());

                var contentMoq = new Mock<HttpContentProvider>();
                contentMoq.Setup(x => x.GetResponseContent(It.IsAny<HttpResponseMessage>()))
                          .Returns(content);

                var handler = new AuthorizeRequestHandler
                {
                    DocumentStore = DocumentStore,
                    InnerHandler = new TestHandler((r, c) => TestHandler.Return200()),
                    ApiKeyProvider = new ApiKeyProvider()
                };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("Referrer", new[] { url });
                client.DefaultRequestHeaders.Add("Referer", new[] { url });

                var result =
                    client.GetAsync("http://webapi/v1/Geocode/326 east south temple/84111?apiKey=key")
                          .Result;

                return (int)result.StatusCode;
            }

            [TestCase("www.example.com", "http://www.example.com/", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/request/test.html", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com", "http://www.badexample.com/", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/request/test.html", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/request/test/index.html?query=yes", ExpectedResult = 400)]

            [TestCase("www.example.com/*", "http://www.example.com/", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/reqes/test.html", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com/*", "http://www.badexample.com/", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/request/test.html", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/request/test/index.html?query=yes", ExpectedResult = 400)]

            [TestCase("www.example.com/", "http://www.example.com", ExpectedResult = 200)]
            [TestCase("www.example.com/", "http://www.example.com/index.html", ExpectedResult = 200)]

            [TestCase("example.com/*", "http://example.com/index.html", ExpectedResult = 200)]
            [TestCase("example.com/*", "http://example.com/request/index.html", ExpectedResult = 200)]

            [TestCase("example.com/*", "http://bad.example.com/index.html", ExpectedResult = 400)]
            [TestCase("example.com/*", "http://bad.example.com/request/index.html", ExpectedResult = 400)]

            [TestCase("*.example.com", "http://any.example.com/", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/index.html", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/request/test.html", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com/test", "http://www.example.com/test/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/test", "http://www.example.com/test", ExpectedResult = 200)]

            [TestCase("www.example.com/test", "http://www.example.com/bad", ExpectedResult = 400)]
            [TestCase("www.example.com/test", "http://www.example.com/bad/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/test", "http://bad.example.com/test/index.html", ExpectedResult = 400)]

            [TestCase("www.example.com/test/*", "http://www.example.com/test/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/test/*", "http://www.example.com/test/test2/index.html", ExpectedResult = 200)]

            [TestCase("www.example.com/test/*", "http://bad.example.com/test/test/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/test/*", "http://www.example.com/bad/test2/index.htm", ExpectedResult = 400)]

            [TestCase("*.nedds.health.utah.gov*", "http://www.nedds.health.utah.gov", ExpectedResult = 200)]
            [TestCase("api.utlegislators.com", "http://api.utlegislators.com", ExpectedResult = 200)]
            [TestCase("*168.177.222.22/app/*", "http://168.177.222.22/app/whatever", ExpectedResult = 200)]
            public int CORS_OriginHeaderIsValidWithReferrerAlso(string pattern, string url)
            {
                //arrange
                const ApiKey.ApplicationType applicationType = ApiKey.ApplicationType.Browser;
                var data = new ApiKeyData
                {
                    AppStatus = ApiKey.ApplicationStatus.Production,
                    UrlPattern = pattern
                };

                using (var s = DocumentStore.OpenSession())
                {
                    s.Store(new ApiKey("key")
                    {
                        AccountId = "testaccount",
                        CreatedAtTicks = 634940675825121039,
                        ApiKeyStatus = ApiKey.KeyStatus.Active,
                        Type = applicationType,
                        AppStatus = ApiKey.ApplicationStatus.Production,
                        RegexPattern =
                            CommandExecutor.ExecuteCommand(new FormatKeyPatternCommand(applicationType, data)),
                        Pattern = null,
                        IsMachineName = false,
                        Deleted = false
                    }, "testkey");

                    s.SaveChanges();
                }

                var content =
                    new ObjectContent<ResultContainer<GeocodeAddressResult>>(new ResultContainer<GeocodeAddressResult>
                    {
                        Result = new GeocodeAddressResult
                        {
                            InputAddress = "tESTING",
                            Score = 100
                        }
                    }, new JsonMediaTypeFormatter());

                content.Headers.Add("X-Type", typeof(ResultContainer<GeocodeAddressResult>).ToString());

                var contentMoq = new Mock<HttpContentProvider>();
                contentMoq.Setup(x => x.GetResponseContent(It.IsAny<HttpResponseMessage>()))
                          .Returns(content);

                var handler = new AuthorizeRequestHandler
                {
                    DocumentStore = DocumentStore,
                    InnerHandler = new TestHandler((r, c) => TestHandler.Return200()),
                    ApiKeyProvider = new ApiKeyProvider()
                };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("Referrer", new[] { url });
                client.DefaultRequestHeaders.Add("Referer", new[] { url });
                client.DefaultRequestHeaders.Add("Origin", new[] { url });

                var result =
                    client.GetAsync("http://webapi/v1/Geocode/326 east south temple/84111?apiKey=key")
                          .Result;

                return (int)result.StatusCode;
            }

            [TestCase("www.example.com", "http://www.example.com/", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/request/test.html", ExpectedResult = 200)]
            [TestCase("www.example.com", "http://www.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com", "http://www.badexample.com/", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/request/test.html", ExpectedResult = 400)]
            [TestCase("www.example.com", "http://www.badexample.com/request/test/index.html?query=yes", ExpectedResult = 400)]

            [TestCase("www.example.com/*", "http://www.example.com/", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/reqes/test.html", ExpectedResult = 200)]
            [TestCase("www.example.com/*", "http://www.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com/*", "http://www.badexample.com/", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/request/test.html", ExpectedResult = 400)]
            [TestCase("www.example.com/*", "http://www.badexample.com/request/test/index.html?query=yes", ExpectedResult = 400)]

            [TestCase("www.example.com/", "http://www.example.com", ExpectedResult = 200)]
            [TestCase("www.example.com/", "http://www.example.com/index.html", ExpectedResult = 200)]

            [TestCase("example.com/*", "http://example.com/index.html", ExpectedResult = 200)]
            [TestCase("example.com/*", "http://example.com/request/index.html", ExpectedResult = 200)]

            [TestCase("example.com/*", "http://bad.example.com/index.html", ExpectedResult = 400)]
            [TestCase("example.com/*", "http://bad.example.com/request/index.html", ExpectedResult = 400)]

            [TestCase("*.example.com", "http://any.example.com/", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/index.html", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/request/test.html", ExpectedResult = 200)]
            [TestCase("*.example.com", "http://any.example.com/request/test/index.html?query=yes", ExpectedResult = 200)]

            [TestCase("www.example.com/test", "http://www.example.com/test/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/test", "http://www.example.com/test", ExpectedResult = 200)]

            [TestCase("www.example.com/test", "http://www.example.com/bad", ExpectedResult = 400)]
            [TestCase("www.example.com/test", "http://www.example.com/bad/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/test", "http://bad.example.com/test/index.html", ExpectedResult = 400)]

            [TestCase("www.example.com/test/*", "http://www.example.com/test/index.html", ExpectedResult = 200)]
            [TestCase("www.example.com/test/*", "http://www.example.com/test/test2/index.html", ExpectedResult = 200)]

            [TestCase("www.example.com/test/*", "http://bad.example.com/test/test/index.html", ExpectedResult = 400)]
            [TestCase("www.example.com/test/*", "http://www.example.com/bad/test2/index.htm", ExpectedResult = 400)]

            [TestCase("*.nedds.health.utah.gov*", "http://www.nedds.health.utah.gov", ExpectedResult = 200)]
            [TestCase("api.utlegislators.com", "http://api.utlegislators.com", ExpectedResult = 200)]
            [TestCase("*168.177.222.22/app/*", "http://168.177.222.22/app/whatever", ExpectedResult = 200)]
            public int CORS_OriginHeaderIsValid(string pattern, string url)
            {
                //arrange
                const ApiKey.ApplicationType applicationType = ApiKey.ApplicationType.Browser;
                var data = new ApiKeyData
                {
                    AppStatus = ApiKey.ApplicationStatus.Production,
                    UrlPattern = pattern
                };

                using (var s = DocumentStore.OpenSession())
                {
                    s.Store(new ApiKey("key")
                    {
                        AccountId = "testaccount",
                        CreatedAtTicks = 634940675825121039,
                        ApiKeyStatus = ApiKey.KeyStatus.Active,
                        Type = applicationType,
                        AppStatus = ApiKey.ApplicationStatus.Production,
                        RegexPattern =
                            CommandExecutor.ExecuteCommand(new FormatKeyPatternCommand(applicationType, data)),
                        Pattern = null,
                        IsMachineName = false,
                        Deleted = false
                    }, "testkey");

                    s.SaveChanges();
                }

                var content =
                    new ObjectContent<ResultContainer<GeocodeAddressResult>>(new ResultContainer<GeocodeAddressResult>
                    {
                        Result = new GeocodeAddressResult
                        {
                            InputAddress = "tESTING",
                            Score = 100
                        }
                    }, new JsonMediaTypeFormatter());

                content.Headers.Add("X-Type", typeof(ResultContainer<GeocodeAddressResult>).ToString());

                var contentMoq = new Mock<HttpContentProvider>();
                contentMoq.Setup(x => x.GetResponseContent(It.IsAny<HttpResponseMessage>()))
                          .Returns(content);

                var handler = new AuthorizeRequestHandler
                {
                    DocumentStore = DocumentStore,
                    InnerHandler = new TestHandler((r, c) => TestHandler.Return200()),
                    ApiKeyProvider = new ApiKeyProvider()
                };

                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("Origin", new[] { url });

                var result =
                    client.GetAsync("http://webapi/v1/Geocode/326 east south temple/84111?apiKey=key")
                          .Result;

                return (int)result.StatusCode;
            }
        }
    }
}