using api.mapserv.utah.gov.Models.Configuration;
using Shouldly;
using Xunit;

namespace api.tests.Models.Configuration {
    public class GisServerConfigurationTests {
        [Fact]
        public void Should_format_url_with_non_standard_port() {
            var config = new GisServerConfiguration {
                Host = "host",
                Port = "1",
                Protocol = "protocol"
            };

            config.ToString().ShouldBe("protocol://host:1");
        }

        [Fact]
        public void Should_omit_standard_port() {
            var config = new GisServerConfiguration {
                Host = "host",
                Port = "80",
                Protocol = "protocol"
            };

            config.ToString().ShouldBe("protocol://host");
        }

        [Fact]
        public void Should_set_good_default() {
            var config = new GisServerConfiguration();

            config.ToString().ShouldBe("http://localhost");
        }
    }
}
