using System;
using System.Collections.Generic;
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

            public PoBoxes() {
                var abbrs = new Abbreviations();
                var regex = new RegexCache(abbrs);

                _handler = new AddressParsing.Handler(regex, abbrs);
            }

            public static IEnumerable<object[]> GetPoBoxes() {
                yield return new object[]
                {
                    new CleansedAddress {
                        InputAddress = "P O Box 123",
                        PoBox = 123,
                        HouseNumber = null,
                        PrefixDirection = Direction.None,
                        StreetName = "P.O. Box",
                        StreetType = StreetType.None,
                        SuffixDirection = Direction.None
                    },
                    "P.O. Box 123",
                    false
                };

                yield return new object[]
                {
                    new CleansedAddress {
                        InputAddress = "PO Box 123",
                        PoBox = 123,
                        HouseNumber = null,
                        PrefixDirection = Direction.None,
                        StreetName = "P.O. Box",
                        StreetType = StreetType.None,
                        SuffixDirection = Direction.None
                    },
                    "P.O. Box 123",
                    false
                };

                yield return new object[]
                {
                    new CleansedAddress {
                        InputAddress = "POBox 123",
                        PoBox = 123,
                        HouseNumber = null,
                        PrefixDirection = Direction.None,
                        StreetName = "P.O. Box",
                        StreetType = StreetType.None,
                        SuffixDirection = Direction.None
                    },
                    "P.O. Box 123",
                    false
                };

                yield return new object[]
                {
                    new CleansedAddress {
                        InputAddress = "P.O. Box 123",
                        PoBox = 123,
                        HouseNumber = null,
                        PrefixDirection = Direction.None,
                        StreetName = "P.O. Box",
                        StreetType = StreetType.None,
                        SuffixDirection = Direction.None
                    },
                    "P.O. Box 123",
                    false
                };
            }

            [Theory]
            [MemberData(nameof(GetPoBoxes))]
            public async Task Should_parse_input_with_spaces(CleansedAddress input, string standardAddress, bool reversal) {
                var request = new AddressParsing.Command(input.InputAddress);
                var result = await _handler.Handle(request, cancellation);

                result.PoBox.ShouldBe(input.PoBox);
                result.HouseNumber.ShouldBe(input.HouseNumber);
                result.PrefixDirection.ShouldBe(input.PrefixDirection);
                result.StreetName.ShouldBe(input.StreetName);
                result.StreetType.ShouldBe(input.StreetType);
                result.SuffixDirection.ShouldBe(input.SuffixDirection);
                result.StandardizedAddress.ShouldBe(standardAddress);
                result.IsReversal().ShouldBe(reversal);
                result.IsPoBox.ShouldBeTrue();
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
