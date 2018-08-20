using api.mapserv.utah.gov.Models;
using Xunit;

namespace api.tests.Filters {
    public class AuthorizeApiKeyTests {
        [Theory]
        [InlineData("www.example.com", "http://www.example.com/", 200)]
        [InlineData("www.example.com", "http://www.example.com/index.html", 200)]
        [InlineData("www.example.com", "http://www.example.com/request/test.html", 200)]
        [InlineData("www.example.com", "http://www.example.com/request/test/index.html?query=yes", 200)]

        [InlineData("www.example.com", "http://www.badexample.com/", 400)]
        [InlineData("www.example.com", "http://www.badexample.com/index.html", 400)]
        [InlineData("www.example.com", "http://www.badexample.com/request/test.html", 400)]
        [InlineData("www.example.com", "http://www.badexample.com/request/test/index.html?query=yes",
            400)]

        [InlineData("www.example.com/*", "http://www.example.com/", 200)]
        [InlineData("www.example.com/*", "http://www.example.com/index.html", 200)]
        [InlineData("www.example.com/*", "http://www.example.com/reqes/test.html", 200)]
        [InlineData("www.example.com/*", "http://www.example.com/request/test/index.html?query=yes", 200)]

        [InlineData("www.example.com/*", "http://www.badexample.com/", 400)]
        [InlineData("www.example.com/*", "http://www.badexample.com/index.html", 400)]
        [InlineData("www.example.com/*", "http://www.badexample.com/request/test.html", 400)]
        [InlineData("www.example.com/*", "http://www.badexample.com/request/test/index.html?query=yes", 400)]

        [InlineData("www.example.com/", "http://www.example.com", 200)]
        [InlineData("www.example.com/", "http://www.example.com/index.html", 200)]

        [InlineData("example.com/*", "http://example.com/index.html", 200)]
        [InlineData("example.com/*", "http://example.com/request/index.html", 200)]

        [InlineData("example.com/*", "http://bad.example.com/index.html", 400)]
        [InlineData("example.com/*", "http://bad.example.com/request/index.html", 400)]

        [InlineData("*.example.com", "http://any.example.com/", 200)]
        [InlineData("*.example.com", "http://any.example.com/index.html", 200)]
        [InlineData("*.example.com", "http://any.example.com/request/test.html", 200)]
        [InlineData("*.example.com", "http://any.example.com/request/test/index.html?query=yes", 200)]

        [InlineData("www.example.com/test", "http://www.example.com/test/index.html", 200)]
        [InlineData("www.example.com/test", "http://www.example.com/test", 200)]

        [InlineData("www.example.com/test", "http://www.example.com/bad", 400)]
        [InlineData("www.example.com/test", "http://www.example.com/bad/index.html", 400)]
        [InlineData("www.example.com/test", "http://bad.example.com/test/index.html", 400)]

        [InlineData("www.example.com/test/*", "http://www.example.com/test/index.html", 200)]
        [InlineData("www.example.com/test/*", "http://www.example.com/test/test2/index.html", 200)]

        [InlineData("www.example.com/test/*", "http://bad.example.com/test/test/index.html", 400)]
        [InlineData("www.example.com/test/*", "http://www.example.com/bad/test2/index.htm", 400)]

        [InlineData("*.nedds.health.utah.gov*", "http://www.nedds.health.utah.gov", 200)]
        [InlineData("api.utlegislators.com", "http://api.utlegislators.com", 200)]
        [InlineData("*168.177.222.22/app/*", "http://168.177.222.22/app/whatever", 200)]
        public void Production_key_is_valid(string pattern, string url, int response) {
            var key = new ApiKey {
                Configuration = ApiKey.ApplicationStatus.Production,
                RegexPattern = pattern
            };


        }
    }
}
