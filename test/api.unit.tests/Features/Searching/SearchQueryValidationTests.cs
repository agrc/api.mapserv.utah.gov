using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;

namespace api.tests.Features.Searching;
public class SearchQueryValidationTests {
    private readonly ILogger _logger;
    private readonly Mock<RequestHandlerDelegate<IApiResponse>> delegateMock;
    private readonly IComputeMediator mediator;
    public SearchQueryValidationTests() {
        _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

        delegateMock = new Mock<RequestHandlerDelegate<IApiResponse>>();
        delegateMock.Setup(x => x());

        var _mediator = new Mock<IComputeMediator>();
        _mediator.Setup(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

        mediator = _mediator.Object;
    }

    // [Fact]
    // public async Task Should_fail_with_no_table_name() {
    //     var query = new SearchQuery.Query(string.Empty, "return,values", new(new()));

    //     var handler = new SearchQuery.ValidationFilter(mediator, _logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("tableName is a required field. Input was empty. ");
    // }

    // [Fact]
    // public async Task Should_fail_with_jerk_table_name() {
    //     var query = new SearchQuery.Query("jerk", "return,values", new(new()));

    //     var _mediator = new Mock<IComputeMediator>();
    //     _mediator.SetupSequence(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
    //             .ReturnsAsync(true)
    //             .ReturnsAsync(false)
    //             .ReturnsAsync(false);

    //     var handler = new SearchQuery.ValidationFilter(_mediator.Object, _logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("tableName contains unsafe characters. Don't be a jerk. ");
    // }

    // [Fact]
    // public async Task Should_fail_with_no_return_values() {
    //     var query = new SearchQuery.Query("table", string.Empty, new(new()));

    //     var handler = new SearchQuery.ValidationFilter(mediator, _logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("returnValues is a required field. Input was empty. ");
    // }

    // [Fact]
    // public async Task Should_fail_with_jerk_return_value() {
    //     var query = new SearchQuery.Query("table", "jerk", new(new()));

    //     var _mediator = new Mock<IComputeMediator>();
    //     _mediator.SetupSequence(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
    //             .ReturnsAsync(false)
    //             .ReturnsAsync(true)
    //             .ReturnsAsync(false);

    //     var handler = new SearchQuery.ValidationFilter(_mediator.Object, _logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("returnValues contains unsafe characters. Don't be a jerk. ");
    // }

    // [Fact]
    // public async Task Should_fail_with_empty_options() {
    //     var query = new SearchQuery.Query("table_not_found", "return,values", null);

    //     var handler = new SearchQuery.ValidationFilter(mediator, _logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("Search options did not bind correctly. Sorry. ");
    // }

    // [Fact]
    // public async Task Should_fail_with_jerk_predicate() {
    //     var query = new SearchQuery.Query("table", "value", new(new SearchRequestOptionsContract {
    //         Predicate = "jerk"
    //     }));

    //     var _mediator = new Mock<IComputeMediator>();
    //     _mediator.SetupSequence(x => x.Handle(It.IsAny<ValidateSql.Computation>(), It.IsAny<CancellationToken>()))
    //             .ReturnsAsync(false)
    //             .ReturnsAsync(false)
    //             .ReturnsAsync(true);

    //     var handler = new SearchQuery.ValidationFilter(_mediator.Object, _logger);
    //     var result = await handler.InvokeAsync(query, delegateMock.Object, CancellationToken.None) as JsonHttpResult<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>;

    //     delegateMock.Verify(x => x(), Times.Never);
    //     result.StatusCode.ShouldBe(400);
    //     result.Value.ShouldBeAssignableTo<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>();

    //     var response = result.Value;
    //     response.Status.ShouldBe(400);
    //     response.Message.ShouldBe("Predicate contains unsafe characters. Don't be a jerk. ");
    // }

    // [Fact]
    // public async Task Should_call_next_with_no_errors() {
    //     var query = new SearchQuery.Query("table_not_found", "return,values", new(new()));

    //     var handler = new SearchQuery.ValidationFilter(mediator, _logger);
    //     var result = await handler.InvokeAsync();

    //     delegateMock.Verify(x => x(), Times.Once);
    // }
}
