using AGRC.api.Models.Configuration;
using Shouldly;
using Xunit;

namespace api.tests.Models.Configuration {
    public class DatabaseConfigurationTests {
        [Fact]
        public void Should_format_connections_string() {
            var model = new DatabaseConfiguration {
                Host = "host",
                Port = "port",
                Db = "db",
                Username = "user",
                Password = "pw"
            };

            model.ConnectionString.ShouldBe("Host=host;Port=port;Username=user;Password=pw;Database=db");
        }
    }
}
