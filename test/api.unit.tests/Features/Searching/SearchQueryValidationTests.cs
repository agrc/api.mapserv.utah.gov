using AGRC.api.Features.Converting;
using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.tests.Features.Searching;
public class SearchQueryValidationTests {
    private readonly ILogger _logger;
    private readonly IComputeMediator _mediator;
    private readonly ApiVersion _version = new(1, 0);
    private readonly IJsonSerializerOptionsFactory _jsonFactory;

    public SearchQueryValidationTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        var mediator = new Mock<IComputeMediator>();
        mediator.Setup(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        _mediator = mediator.Object;
        var jsonFactory = new Mock<IJsonSerializerOptionsFactory>();
        jsonFactory.Setup(x => x.GetSerializerOptionsFor(It.IsAny<ApiVersion>())).Returns(new JsonSerializerOptions());

        _jsonFactory = jsonFactory.Object;
    }
    [Fact]
    public async Task Should_fail_with_no_table_name() {
        var context = HttpContextHelpers.GetEndpointContext(string.Empty, "return,values", new SearchOptions(new()));

        var filter = new SearchQuery.ValidationFilter(_mediator, _jsonFactory, _version, _logger);
        var result = await filter.InvokeAsync(context, (_) => new ValueTask<object>()) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

        result.StatusCode.ShouldBe(400);
        result.Value.Status.ShouldBe(400);
        result.Value.Message.ShouldBe("tableName is a required field. Input was empty. ");
    }

    [Fact]
    public async Task Should_fail_with_jerk_table_name() {
        var context = HttpContextHelpers.GetEndpointContext("jerk", "return,values", new SearchOptions(new()));

        var _mediator = new Mock<IComputeMediator>();
        _mediator.SetupSequence(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false)
                .ReturnsAsync(false);

        var filter = new SearchQuery.ValidationFilter(_mediator.Object, _jsonFactory, _version, _logger);
        var result = await filter.InvokeAsync(context, (_) => new ValueTask<object>()) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

        result.StatusCode.ShouldBe(400);
        result.Value.Status.ShouldBe(400);
        result.Value.Message.ShouldBe("tableName contains unsafe characters. Don't be a jerk. ");
    }

    [Fact]
    public async Task Should_fail_with_no_return_values() {
        var context = HttpContextHelpers.GetEndpointContext("table", string.Empty, new SearchOptions(new()));

        var filter = new SearchQuery.ValidationFilter(_mediator, _jsonFactory, _version, _logger);
        var result = await filter.InvokeAsync(context, (_) => new ValueTask<object>()) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

        result.StatusCode.ShouldBe(400);
        result.Value.Status.ShouldBe(400);
        result.Value.Message.ShouldBe("returnValues is a required field. Input was empty. ");
    }

    [Fact]
    public async Task Should_fail_with_jerk_return_value() {
        var context = HttpContextHelpers.GetEndpointContext("table", "jerk", new SearchOptions(new()));

        var _mediator = new Mock<IComputeMediator>();
        _mediator.SetupSequence(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .ReturnsAsync(true)
                .ReturnsAsync(false);

        var filter = new SearchQuery.ValidationFilter(_mediator.Object, _jsonFactory, _version, _logger);
        var result = await filter.InvokeAsync(context, (_) => new ValueTask<object>()) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

        result.StatusCode.ShouldBe(400);
        result.Value.Status.ShouldBe(400);
        result.Value.Message.ShouldBe("returnValues contains unsafe characters. Don't be a jerk. ");
    }

    [Fact]
    public async Task Should_fail_with_empty_options() {
        var context = HttpContextHelpers.GetEndpointContext("table_not_found", "return,values", null);

        var filter = new SearchQuery.ValidationFilter(_mediator, _jsonFactory, _version, _logger);
        var result = await filter.InvokeAsync(context, (_) => new ValueTask<object>()) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

        result.StatusCode.ShouldBe(400);
        result.Value.Status.ShouldBe(400);
        result.Value.Message.ShouldBe("Search options did not bind correctly. Sorry. ");
    }

    [Fact]
    public async Task Should_fail_with_jerk_predicate() {
        var context = HttpContextHelpers.GetEndpointContext("table", "value", new SearchOptions(new SearchRequestOptionsContract {
            Predicate = "jerk"
        }));

        var _mediator = new Mock<IComputeMediator>();
        _mediator.SetupSequence(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .ReturnsAsync(false)
                .ReturnsAsync(true);

        var filter = new SearchQuery.ValidationFilter(_mediator.Object, _jsonFactory, _version, _logger);
        var result = await filter.InvokeAsync(context, (_) => new ValueTask<object>()) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

        result.StatusCode.ShouldBe(400);
        result.Value.Status.ShouldBe(400);
        result.Value.Message.ShouldBe("Predicate contains unsafe characters. Don't be a jerk. ");
    }
}
