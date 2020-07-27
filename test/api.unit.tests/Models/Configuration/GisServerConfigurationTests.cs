using AGRC.api.Models.Configuration;
using Shouldly;
using Xunit;

namespace api.tests.Models.Configuration {
    public class LocatorConfigurationTests {
        [Fact]
        public void Should_format_url_with_non_standard_port() {
            var config = new LocatorConfiguration {
                Host = "host",
                Port = "1",
                Protocol = "protocol"
            };

            config.GetHost().ShouldBe("protocol://host:1");
        }

        [Fact]
        public void Should_omit_standard_port() {
            var config = new LocatorConfiguration {
                Host = "host",
                Port = "80",
                Protocol = "protocol"
            };

            config.GetHost().ShouldBe("protocol://host");
        }

        [Fact]
        public void Should_set_good_default() {
            var config = new LocatorConfiguration();

            config.GetHost().ShouldBe("http://localhost");
        }
    }
}
