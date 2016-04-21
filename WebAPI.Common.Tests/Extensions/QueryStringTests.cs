using NUnit.Framework;
using Newtonsoft.Json;
using WebAPI.Common.Extensions;

namespace WebAPI.Common.Tests.Extensions
{
    [TestFixture]
    public class QueryStringTests
    {
        public class TestMe
        {
            public string A { get; set; }
            public double B { get; set; }
        }

        private class JsonIgnore : TestMe
        {
            [JsonIgnore]
            public string C { get; set; }
        }

        public class JsonPropertyName
        {
            [JsonProperty(PropertyName="a")]
            public string A { get; set; }

            [JsonProperty(PropertyName = "b")]
            public double B { get; set; }
        }

        [Test]
        public void CreatesEmptyQueryParamForNullAndEmptyValues()
        {
            var thing = new TestMe();
            Assert.That(thing.ToQueryString(), Is.EqualTo("A=&B=0"));
        }

        [Test]
        public void UsesJsonNetPropertyNames()
        {
            var thing = new JsonPropertyName();
            Assert.That(thing.ToQueryString(), Is.EqualTo("a=&b=0"));
        }

        [Test]
        public void IgnoresPropertiesMarkedWithJsonIgnore()
        {
            var thing = new JsonIgnore();
            Assert.That(thing.ToQueryString(), Is.EqualTo("A=&B=0"));
        }
    }
}