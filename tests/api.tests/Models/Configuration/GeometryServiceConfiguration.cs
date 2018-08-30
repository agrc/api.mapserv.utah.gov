using api.mapserv.utah.gov.Models.Configuration;
using Shouldly;
using Xunit;

namespace api.tests.Models.Configuration {
    public class GeometryServiceConfigurationTests {
        [Fact]
        public void Should_format_url_with_non_standard_port() {
            var model = new GeometryServiceConfiguration {
                Host = "host",
                Port = "1",
                Path = "path",
                Protocol = "protocol"
            };

            model.ToString().ShouldBe("protocol://host:1/path/project{0}");
        }

        [Fact]
        public void Should_omit_standard_port() {
            var model = new GeometryServiceConfiguration {
                Host = "host",
                Port = "80",
                Path = "path",
                Protocol = "protocol"
            };

            model.ToString().ShouldBe("protocol://host/path/project{0}");
        }

        [Fact]
        public void Should_remove_double_slashes() {
            var model = new GeometryServiceConfiguration {
                Host = "host",
                Port = "80",
                Path = "/path/",
                Protocol = "protocol"
            };

            model.ToString().ShouldBe("protocol://host/path/project{0}");
        }

        [Fact]
        public void Should_set_good_default() {
            var model = new GeometryServiceConfiguration();

            model.ToString().ShouldBe("http://localhost/arcgis/rest/services/Geometry/GeometryServer/project{0}");
        }
    }
}
