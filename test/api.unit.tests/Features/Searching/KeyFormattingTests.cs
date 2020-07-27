using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Searching;
using api.mapserv.utah.gov.Models.Constants;
using MediatR;
using Shouldly;
using Xunit;

namespace api.tests.Features.Searching {
    public class KeyFormattingTests {
        private readonly KeyFormatting.Pipeline<SqlQuery.Command, IReadOnlyCollection<SearchResponseContract>> _handler;
        private readonly IReadOnlyCollection<SearchResponseContract> _data;

        public KeyFormattingTests() {
            _data = new List<SearchResponseContract>{
                new SearchResponseContract {
                    Attributes = new Dictionary<string, object>() {
                        { "UPPER", 0 },
                        { "MixeD", 0 },
                        { "lower", 0 }
                    }
                }
            };

            _handler = new KeyFormatting.Pipeline<SqlQuery.Command, IReadOnlyCollection<SearchResponseContract>>();
        }

        [Fact]
        public async Task Should_lowercase_all_keys() {
            var command = new SqlQuery.Command("tablename", "attributes", "query", AttributeStyle.Lower);

            var result = await _handler.Handle(command, CancellationToken.None, () => Task.FromResult(_data));

            result.First().Attributes.Count.ShouldBe(3);
            result.First().Attributes.Keys.All(x => x.All(char.IsLower)).ShouldBe(true);
        }

        [Fact]
        public async Task Should_uppercase_all_keys() {
            var command = new SqlQuery.Command("tablename", "attributes", "query", AttributeStyle.Upper);

            var result = await _handler.Handle(command, CancellationToken.None, () => Task.FromResult(_data));

            result.First().Attributes.Count.ShouldBe(3);
            result.First().Attributes.Keys.All(x => x.All(char.IsUpper)).ShouldBe(true);
        }

        [Fact]
        public async Task Should_keep_all_keys_as_is() {
            var command = new SqlQuery.Command("tablename", "attributes", "query", AttributeStyle.Input);
            var result = await _handler.Handle(command, CancellationToken.None, () => Task.FromResult(_data));

            result.First().Attributes.Count.ShouldBe(3);
            result.ShouldBeSameAs(_data);
        }
    }
}
