using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Searching;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Searching {
    public class TableMappingTests {
        private readonly IReadOnlyCollection<SearchResponseContract> _data;
        private readonly IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>> _computationHandler;
        private readonly ILogger _logger;
        private SqlQuery.Computation _mutation;

        public TableMappingTests() {
            _data = new List<SearchResponseContract>{
                new SearchResponseContract {
                    Attributes = new Dictionary<string, object>() {
                        { "UPPER", 0 },
                        { "MixeD", 0 },
                        { "lower", 0 }
                    }
                }
            };

            _logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;

            var handler = new Mock<IComputationHandler<SqlQuery.Computation, IReadOnlyCollection<SearchResponseContract>>>();
            handler.Setup(x => x.Handle(It.IsAny<SqlQuery.Computation>(),
                                        It.IsAny<CancellationToken>()))
                   .Callback<SqlQuery.Computation, CancellationToken>((comp, token) => {
                       _mutation = comp;
                   })
                   .ReturnsAsync(_data);
            _computationHandler = handler.Object;
        }

        [Fact]
        public async Task Should_swap_sgid_table() {
            var computation = new SqlQuery.Computation("sgid.category.table", "attributes", "query", AttributeStyle.Lower);

            var tableMapping = new Mock<ITableMapping>();
            tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "category.table", "swapped" } });

            var decorator = new SqlQuery.TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

            var _ = await decorator.Handle(computation, CancellationToken.None);

            _mutation.TableName.ShouldBe("swapped");
            _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
            _mutation.Predicate.ShouldBe(computation.Predicate);
            _mutation.Styling.ShouldBe(computation.Styling);
        }

        [Fact]
        public async Task Should_skip_non_sgid_tables() {
            var computation = new SqlQuery.Computation("tablename", "attributes", "query", AttributeStyle.Upper);

            var tableMapping = new Mock<ITableMapping>();
            tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "not-found", "value" } });

            var decorator = new SqlQuery.TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

            var _ = await decorator.Handle(computation, CancellationToken.None);

            _mutation.TableName.ShouldBe(computation.TableName);
            _mutation.ReturnValues.ShouldBe(computation.ReturnValues);
            _mutation.Predicate.ShouldBe(computation.Predicate);
            _mutation.Styling.ShouldBe(computation.Styling);
        }

        [Fact]
        public async Task Should_throw_if_table_does_not_exist() {
            var computation = new SqlQuery.Computation("sgid.layer.does-not-exist", "attributes", "query", AttributeStyle.Upper);

            var tableMapping = new Mock<ITableMapping>();
            tableMapping.SetupGet(x => x.MsSqlToPostgres).Returns(new Dictionary<string, string> { { "key", "value" } });

            var decorator = new SqlQuery.TableMappingDecorator(_computationHandler, tableMapping.Object, _logger);

            var _ = await decorator.Handle(computation, CancellationToken.None).ShouldThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public void Should_contain_key() {
            var mapping = new TableMapping();
            mapping.MsSqlToPostgres["boundaries.counties"].ShouldBe("boundaries.county_boundaries");
        }
    }
}
