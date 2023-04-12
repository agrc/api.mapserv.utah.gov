using AGRC.api.Features.Searching;

namespace api.tests.Features.Searching;
public class SearchProviderConfigurationTests {
    [Fact]
    public void Should_build_connection_string() {
        var connection = new SearchProviderConfiguration {
            Host = "host",
            Db = "db",
            Username = "username",
            Password = "password"
        };

        connection.ConnectionString.ShouldBe("Host=host;Username=username;Password=password;Database=db;Timeout=5;SslMode=Disable;");
    }
}
