using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using MediatR;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class AddressParsingTests {
        public class PoBoxes {
            IRequestHandler<AddressParsing.Command, CleansedAddress> _handler;
            CancellationToken cancellation = new CancellationToken();

            public PoBoxes()
            {
                var abbrs = new Abbreviations();
                var regex = new RegexCache(abbrs);

                _handler = new AddressParsing.Handler(regex, abbrs);
            }

            [Fact]
            public async Task Should_parse_input_with_spaces() {
                var request = new AddressParsing.Command("P O Box 123");
                var result = await _handler.Handle(request, cancellation);

                result.PoBox.ShouldBe(123);
                result.HouseNumber.ShouldBeNull();
                result.PrefixDirection.ShouldBe(Direction.None);
                result.StreetName.ShouldBe("P.O. Box");
                result.StreetType.ShouldBe(StreetType.None);
                result.SuffixDirection.ShouldBe(Direction.None);
                result.StandardizedAddress.ShouldBe("P.O. Box 123");
                result.IsReversal().ShouldBeFalse();
            }

            [Fact]
            public async Task Should_parse_input_without_spaces() {
                var request = new AddressParsing.Command("PO Box 123");
                var result = await _handler.Handle(request, cancellation);

                result.IsPoBox.ShouldBeTrue();
                result.PoBox.ShouldBe(123);
                result.HouseNumber.ShouldBeNull();
                result.PrefixDirection.ShouldBe(Direction.None);
                result.StreetName.ShouldBe("P.O. Box");
                result.StreetType.ShouldBe(StreetType.None);
                result.SuffixDirection.ShouldBe(Direction.None);
                result.StandardizedAddress.ShouldBe("P.O. Box 123");
                result.IsReversal().ShouldBeFalse();
            }

            [Fact]
            public async Task Should_parse_input_with_no_spaces() {
                var request = new AddressParsing.Command("POBox 123");
                var result = await _handler.Handle(request, cancellation);

                result.IsPoBox.ShouldBeTrue();
                result.PoBox.ShouldBe(123);
                result.HouseNumber.ShouldBeNull();
                result.PrefixDirection.ShouldBe(Direction.None);
                result.StreetName.ShouldBe("P.O. Box");
                result.StreetType.ShouldBe(StreetType.None);
                result.SuffixDirection.ShouldBe(Direction.None);
                result.StandardizedAddress.ShouldBe("P.O. Box 123");
                result.IsReversal().ShouldBeFalse();
            }

            [Fact]
            public async Task Should_parse_input_with_period_separation() {
                var request = new AddressParsing.Command("P.O.Box 123");
                var result = await _handler.Handle(request, cancellation);

                result.IsPoBox.ShouldBeTrue();
                result.PoBox.ShouldBe(123);
                result.HouseNumber.ShouldBeNull();
                result.PrefixDirection.ShouldBe(Direction.None);
                result.StreetName.ShouldBe("P.O. Box");
                result.StreetType.ShouldBe(StreetType.None);
                result.SuffixDirection.ShouldBe(Direction.None);
                result.StandardizedAddress.ShouldBe("P.O. Box 123");
                result.IsReversal().ShouldBeFalse();
            }

            [Fact]
            public async Task Should_not_flag_house_address_as_pobox() {
                var request = new AddressParsing.Command("123 west house st");
                var result = await _handler.Handle(request, cancellation);

                result.IsPoBox.ShouldBeFalse();
                result.PoBox.ShouldBe(0);
                result.HouseNumber.ShouldBe(123);
                result.PrefixDirection.ShouldBe(Direction.West);
                result.StreetName.ShouldBe("house");
                result.StreetType.ShouldBe(StreetType.Street);
                result.SuffixDirection.ShouldBe(Direction.None);
                result.StandardizedAddress.ToLowerInvariant().ShouldBe("123 west house street");
                result.IsReversal().ShouldBeFalse();
            }
        }
    }
}
