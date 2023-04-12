using AGRC.api.Features.Searching;

namespace api.tests.Features.Searching;
public class ValidateSqlTests {
    private readonly ValidateSql.Handler handler;

    public ValidateSqlTests() {
        handler = new ValidateSql.Handler();
    }
    [Fact]
    public async Task Should_return_false_for_harmless_sql() {
        var computation = new ValidateSql.Computation("select * from table");

        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBe(false);
    }

    [Fact]
    public async Task Should_return_true_for_harmful_sql() {
        var computation = new ValidateSql.Computation("select * from table; drop table table");

        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBe(true);
    }
}
