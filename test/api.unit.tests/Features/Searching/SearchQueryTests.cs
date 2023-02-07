using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ResponseContracts;
using Moq;
using Npgsql;
using Serilog;
using Shouldly;
using Xunit;
using static AGRC.api.Features.Converting.EsriGraphic;

namespace api.tests.Features.Searching {
    public class SearchQueryTests {
        private readonly ILogger _logger;
        public SearchQueryTests() {
            _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
        }

        [Fact]
        public async Task Should_return_results() {
            var query = new SearchQuery.Query("tableName", "return,values", new SearchRequestOptionsContract());

            var mediator = new Mock<IComputeMediator>();
            mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new[] {
                        new SearchResponseContract {
                            Geometry = new SerializableGraphic(
                                new EsriJson.Net.Graphic(
                                    new EsriJson.Net.Geometry.Point(1, 1),
                                    new Dictionary<string, object>()
                                )),
                            Attributes = new Dictionary<string, object> {
                                { "key", "value"}
                            }
                        }
                    });

            var handler = new SearchQuery.Handler(mediator.Object, _logger);
            var result = await handler.Handle(query, CancellationToken.None);

            result.StatusCode.ShouldBe(200);
            result.Value.ShouldBeAssignableTo<ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>>();

            var response = result.Value as ApiResponseContract<IReadOnlyCollection<SearchResponseContract>>;
            response.Status.ShouldBe(200);
            response.Result.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Should_handle_sgid_table_not_found() {
            var query = new SearchQuery.Query("table_not_found", "return,values", new SearchRequestOptionsContract());

            var mediator = new Mock<IComputeMediator>();
            mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new KeyNotFoundException());

            var handler = new SearchQuery.Handler(mediator.Object, _logger);
            var result = await handler.Handle(query, CancellationToken.None);

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeAssignableTo<ApiResponseContract<SearchResponseContract>>();

            var response = result.Value as ApiResponseContract<SearchResponseContract>;
            response.Status.ShouldBe(400);
            response.Message.ShouldBe("The table `TABLE_NOT_FOUND` does not exist in the SGID. Please read https://gis.utah.gov/sgid-product-relaunch-update/#static-sgid-data-layers for more information.");
        }

        [Fact]
        public async Task Should_handle_pg_table_not_found() {
            var query = new SearchQuery.Query("table_not_found", "return,values", new SearchRequestOptionsContract());

            var mediator = new Mock<IComputeMediator>();
            mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new PostgresException("relation \"table_not_found\" does not exist", "ERROR", "ERROR", "42P01"));

            var handler = new SearchQuery.Handler(mediator.Object, _logger);
            var result = await handler.Handle(query, CancellationToken.None);

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeAssignableTo<ApiResponseContract<SearchResponseContract>>();

            var response = result.Value as ApiResponseContract<SearchResponseContract>;
            response.Status.ShouldBe(400);
            response.Message.ShouldBe("The table `TABLE_NOT_FOUND` does not exist in the Open SGID.");
        }

        [Fact]
        public async Task Should_handle_invalid_columns() {
            var query = new SearchQuery.Query("table_not_found", "not_found", new SearchRequestOptionsContract());

            var mediator = new Mock<IComputeMediator>();
            mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new PostgresException("column \"not_found\" does not exist", "ERROR", "ERROR", "42703"));

            var handler = new SearchQuery.Handler(mediator.Object, _logger);
            var result = await handler.Handle(query, CancellationToken.None);

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeAssignableTo<ApiResponseContract<SearchResponseContract>>();

            var response = result.Value as ApiResponseContract<SearchResponseContract>;
            response.Status.ShouldBe(400);
            response.Message.ShouldBe("column `not_found` does not exist on `TABLE_NOT_FOUND`. Check that the fields exist.");
        }

        [Fact]
        public async Task Should_handle_invalid_predicate() {
            var query = new SearchQuery.Query("table", "field", new SearchRequestOptionsContract {
                Predicate = "bad predicate"
            });

            var mediator = new Mock<IComputeMediator>();
            mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new PostgresException("argument of WHERE must be type boolean, not type character varying", "ERROR", "ERROR", "42804"));

            var handler = new SearchQuery.Handler(mediator.Object, _logger);
            var result = await handler.Handle(query, CancellationToken.None);

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeAssignableTo<ApiResponseContract<SearchResponseContract>>();

            var response = result.Value as ApiResponseContract<SearchResponseContract>;
            response.Status.ShouldBe(400);
            response.Message.ShouldBe("`BAD PREDICATE` is not a valid T-SQL where clause.");
        }

        [Fact]
        public async Task Should_handle_unhandled_pg_exception() {
            var query = new SearchQuery.Query("table", "field", new SearchRequestOptionsContract {
                Predicate = "bad predicate"
            });

            var mediator = new Mock<IComputeMediator>();
            mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new PostgresException("unknown", "ERROR", "ERROR", "unknown"));

            var handler = new SearchQuery.Handler(mediator.Object, _logger);
            var result = await handler.Handle(query, CancellationToken.None);

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeAssignableTo<ApiResponseContract<SearchResponseContract>>();

            var response = result.Value as ApiResponseContract<SearchResponseContract>;
            response.Status.ShouldBe(400);
            response.Message.ShouldBe("unknown");
        }

        [Fact]
        public async Task Should_handle_unhandled_exception() {
            var query = new SearchQuery.Query("table", "field", new SearchRequestOptionsContract {
                Predicate = "bad predicate"
            });

            var mediator = new Mock<IComputeMediator>();
            mediator.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception("unknown"));

            var handler = new SearchQuery.Handler(mediator.Object, _logger);
            var result = await handler.Handle(query, CancellationToken.None);

            result.StatusCode.ShouldBe(400);
            result.Value.ShouldBeAssignableTo<ApiResponseContract<SearchResponseContract>>();

            var response = result.Value as ApiResponseContract<SearchResponseContract>;
            response.Status.ShouldBe(400);
            response.Message.ShouldBe("The table `TABLE` might not exist. Check your spelling.");
        }
    }
}
