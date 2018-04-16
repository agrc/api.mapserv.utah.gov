using System.Net.Http;
using NUnit.Framework;
using WebAPI.Common.Providers;

namespace WebAPI.Common.Tests.Providers
{
    public class ApiKeyProviderTests
    {
        [TestCase("http://webapi/api/v1/geocode/ago/agrc-ago/geocodeserver?apikey=nospace", "nospace")]
        [TestCase("http://webapi/api/v1/geocode/ago/agrc-ago/geocodeserver?apikey= frontspace", "frontspace")]
        [TestCase("http://webapi/api/v1/geocode/ago/agrc-ago/geocodeserver?apikey=backspace ", "backspace")]
        [TestCase("http://webapi/api/v1/geocode/ago/agrc-ago/geocodeserver?apikey= bothspace ", "bothspace")]
        public void GetsApiKeyFromRequest(string url, string key)
        {
            var apiKeyProvider = new ApiKeyProvider();

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            var apikey = apiKeyProvider.GetApiFromRequestAsync(request).Result;

            Assert.That(apikey, Is.EqualTo(key));
        }
    }
}
