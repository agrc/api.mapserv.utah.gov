using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;
using Serilog;

namespace api.tests.Features.Geocoding {
    public class AddressParsingTests {
        public AddressParsingTests() {
            var abbreviations = new Abbreviations();
            var regex = new RegexCache(abbreviations);

            var mock = new Mock<ILogger> { DefaultValue = DefaultValue.Mock };

            _handler = new AddressParsing.Handler(regex, abbreviations, mock.Object);
        }

        private readonly IComputationHandler<AddressParsing.Computation, Address> _handler;

        public static IEnumerable<object[]> GetPoBoxes() {
            yield return new object[] {
                Address.BuildPoBoxAddress("P O Box 123", 123, 0),
                "P.O. Box 123",
                false
            };

            yield return new object[] {
                Address.BuildPoBoxAddress("PO Box 123", 123, 0),
                "P.O. Box 123",
                false
            };

            yield return new object[] {
                Address.BuildPoBoxAddress("POBox 123", 123, 0),
                "P.O. Box 123",
                false
            };

            yield return new object[] {
                Address.BuildPoBoxAddress("P.O. Box 123", 123, 0),
                "P.O. Box 123",
                false
            };
        }

        public static IEnumerable<object[]> UnitsWithNoTypeDesignation() {
            yield return new object[] {
                new Address("625 NORTH REDWOOD ROAD 6", 625, Direction.North, "REDWOOD", StreetType.Road, Direction.None, null, 0, null, 0, false, false),
                "625 north redwood road"
            };

            yield return new object[] {
                new Address("295 North 120 West 1", 295, Direction.North, "120", StreetType.None, Direction.West, null, 0, null, 0, false, false),
                "295 north 120 west"
            };
        }

        public static IEnumerable<object[]> AbbreviatedHighways() {
            yield return new object[] {
                new Address("401 N HWY 68", 401, Direction.North, "Highway 68", StreetType.Highway, Direction.None,null, 0, null, 0, false, true),
                "401 north highway 68"
            };

            yield return new object[] {
                new Address("401 N SR 68", 401, Direction.North, "Highway 68", StreetType.Highway, Direction.None, null, 0, null, 0, false, true),
                "401 north highway 68"
                };

            yield return new object[] {
                new Address("401 N US 89", 401, Direction.North, "Highway 89", StreetType.Highway, Direction.None, null, 0, null, 0, false, true),
                "401 north highway 89"
            };

            yield return new object[] {
                new Address("401 N U.S. 89", 401, Direction.North, "Highway 89", StreetType.Highway, Direction.None, null, 0, null, 0, false, true),
                "401 north highway 89"
            };
        }

        public static IEnumerable<object[]> EnumsInStreetName() {
            yield return new object[] {
                new Address("4463 SUMMER PLACE CIR", 4463, Direction.None, "SUMMER PLACE", StreetType.Circle, Direction.None, null, 0, null, 0, false, false),
                "4463 summer place circle"
            };

            yield return new object[] {
                new Address("4463 SUMMER PL CIR", 4463, Direction.None, "SUMMER PL", StreetType.Circle, Direction.None, null, 0, null, 0, false, false),
                "4463 summer pl circle"
            };

            yield return new object[] {
                new Address("478 S WEST FRONTAGE RD", 478, Direction.South, "WEST FRONTAGE", StreetType.Road, Direction.None, null, 0, null, 0, false, false),
                "478 south west frontage road"
            };

            yield return new object[] {
                new Address("5301 w jacob hill cir", 5301, Direction.West, "jacob hill", StreetType.Circle, Direction.None, null, 0, null, 0, false, false),
                "5301 west jacob hill circle"
            };

            yield return new object[] {
                new Address("5811 W Park West Rd", 5811, Direction.West, "Park West", StreetType.Road, Direction.None, null, 0, null, 0, false, false),
                "5811 west park west road"
            };

            yield return new object[] {
                new Address("6112 W Whistle Stop Road", 6112, Direction.West, "Whistle Stop", StreetType.Road, Direction.None, null, 0, null, 0, false, false),
                "6112 west whistle stop road"
            };
        }

        public static IEnumerable<object[]> SecondaryUnits() {
            yield return new object[] {
                new Address("435 N MAIN ST bsmt", 435, Direction.North, "MAIN", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "435 north main street"
            };

            yield return new object[] {
                new Address("435 N MAIN ST PENTHOUSE", 435, Direction.North, "MAIN", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "435 north main street"
            };

            yield return new object[] {
                new Address("436 N MAIN ST APT 7", 436, Direction.North, "MAIN", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "436 north main street"
            };

            yield return new object[] {
                new Address("3700 W 150 N LOT 122", 3700, Direction.West, "150", StreetType.None, Direction.North, null, 0, null, 0, false, false),
                "3700 west 150 north"
            };

            yield return new object[] {
                new Address("222 N 1200 W TRLR 44", 222, Direction.North, "1200", StreetType.None, Direction.West, null, 0, null, 0, false, false),
                "222 north 1200 west"
            };

            yield return new object[] {
                new Address("1015 S RIVER RD UNIT 42", 1015, Direction.South, "RIVER", StreetType.Road, Direction.None, null, 0, null, 0, false, false),
                "1015 south river road"
            };

            yield return new object[] {
                new Address("859 PINEWAY DR APT 11E", 859, Direction.None, "PINEWAY", StreetType.Drive, Direction.None, null, 0, null, 0, false, false),
                "859 pineway drive"
            };

            yield return new object[] {
                new Address("902 E 500 S APT F", 902, Direction.East, "500", StreetType.None, Direction.South, null, 0, null, 0, false, false),
                "902 east 500 south"
            };

            yield return new object[] {
                new Address("1388 W 699 S Bsmt", 1388, Direction.West, "699", StreetType.None, Direction.South, null, 0, null, 0, false, false),
                "1388 west 699 south"
            };

            yield return new object[] {
                new Address("1389 W 699 S Suite 900", 1389, Direction.West, "699", StreetType.None, Direction.South, null, 0, null, 0, false, false),
                "1389 west 699 south"
            };

            yield return new object[] {
                new Address("1490 E FOREMASTER DR STE 150", 1490, Direction.East, "FOREMASTER", StreetType.Drive, Direction.None, null, 0, null, 0, false, false),
                "1490 east foremaster drive"
            };

            yield return new object[] {
                new Address("1387 W 699 S Ste 900", 1387, Direction.West, "699", StreetType.None, Direction.South, null, 0, null, 0, false, false),
                "1387 west 699 south"
            };

            yield return new object[] {
                new Address("1387 W 699 S Ste B150", 1387, Direction.West, "699", StreetType.None, Direction.South, null, 0, null, 0, false, false),
                "1387 west 699 south"
            };
        }

        public static IEnumerable<object[]> Concatenations() {
            yield return new object[] {
                new Address("1991S 800E", 1991, Direction.South, "800", StreetType.None, Direction.East, null, 0, null, 0, false, false),
                "1991 south 800 east"
            };

            yield return new object[] {
                new Address("1991S 900 E", 1991, Direction.South, "900", StreetType.None, Direction.East, null, 0, null, 0, false, false),
                "1991 south 900 east"
            };

            yield return new object[] {
                new Address("1991 S 900E", 1991, Direction.South, "900", StreetType.None, Direction.East, null, 0, null, 0, false, false),
                "1991 south 900 east"
            };
        }

        public static IEnumerable<object[]> NonStandardNumericStreetNames() {
            yield return new object[] {
                new Address("1048 W 1205 N", 1048, Direction.West, "1205", StreetType.None, Direction.North, null, 0, null, 0, false, false),
                "1048 west 1205 north"
            };

            yield return new object[] {
                new Address("2139 N 50 W", 2139, Direction.North, "50", StreetType.None, Direction.West, null, 0, null, 0, false, false),
                "2139 north 50 west"
            };
        }

        public static IEnumerable<object[]> OneCharacterStreetNameOrHouseNumber() {
            yield return new object[] {
                new Address("168 N ST", 168, Direction.None, "N", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "168 n street"
            };

            yield return new object[] {
                new Address("168 N N ST", 168, Direction.North, "N", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "168 north n street"
            };

            yield return new object[] {
                new Address("5 Cedar Ave", 5, Direction.None, "Cedar", StreetType.Avenue, Direction.None, null, 0, null, 0, false, false),
                "5 cedar avenue"
            };
        }

        public static IEnumerable<object[]> OrdinalStreetNames() {
            yield return new object[] {
                new Address("1238 E 1ST Avenue", 1238, Direction.East, "1ST", StreetType.Avenue, Direction.None, null, 0, null, 0, false, false),
                "1238 east 1st avenue"
            };

            yield return new object[] {
                new Address("1238 E FIRST Avenue", 1238, Direction.East, "FIRST", StreetType.Avenue, Direction.None, null, 0, null, 0, false, false),
                "1238 east first avenue"
            };

            yield return new object[] {
                new Address("1238 E 2ND Avenue", 1238, Direction.East, "2ND", StreetType.Avenue, Direction.None, null, 0, null, 0, false, false),
                "1238 east 2nd avenue"
            };

            yield return new object[] {
                new Address("1238 E 3RD Avenue", 1238, Direction.East, "3RD", StreetType.Avenue, Direction.None, null, 0, null, 0, false, false),
                "1238 east 3rd avenue"
            };

            yield return new object[] {
                new Address("1573 24TH Street", 1573, Direction.None, "24TH", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "1573 24th street"
            };
        }

        public static IEnumerable<object[]> OriginalTestAddresses() {
            yield return new object[] {
                new Address("400 S 532 E", 400, Direction.South, "532", StreetType.None, Direction.East, null, 0, null, 0, false, false),
                true,
                false,
                true,
                true
            };

            yield return new object[] {
                new Address("5625 S 995 E", 5625, Direction.South, "995", StreetType.None, Direction.East, null, 0, null, 0, false, false),
                false,
                true,
                true,
                true
            };

            yield return new object[] {
                new Address("372 North 600 East", 372, Direction.North, "600", StreetType.None, Direction.East, null, 0, null, 0, false, false),
                false,
                false,
                true,
                true
            };

            yield return new object[] {
                new Address("30 WEST 300 NORTH", 30, Direction.West, "300", StreetType.None, Direction.North, null, 0, null, 0, false, false),
                false,
                true,
                true,
                true
            };

            yield return new object[] {
                new Address("126 E 400 N", 126, Direction.East, "400", StreetType.None, Direction.North, null, 0, null, 0, false, false),
                false,
                false,
                true,
                true
            };

            yield return new object[] {
                new Address("270 South 1300 East", 270, Direction.South, "1300", StreetType.None, Direction.East, null, 0, null, 0, false, false),
                false,
                true,
                true,
                true
            };

            yield return new object[] {
                new Address("126 W SEGO LILY DR", 126, Direction.West, "SEGO LILY", StreetType.Drive, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new Address("261 E MUELLER PARK RD", 261, Direction.East, "MUELLER PARK", StreetType.Road, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new Address("17 S VENICE MAIN ST", 17, Direction.South, "VENICE MAIN", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new Address("20 W Center St", 20, Direction.West, "Center", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new Address("9314 ALVEY LN", 9314, Direction.None, "ALVEY", StreetType.Lane, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                false,
                false
            };

            yield return new object[] {
                new Address("167 DALY AVE", 167, Direction.None, "DALY", StreetType.Avenue, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                false,
                false
            };

            yield return new object[] {
                new Address("1147 MCDANIEL CIR", 1147, Direction.None, "MCDANIEL", StreetType.Circle, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                false,
                false
            };

            yield return new object[] {
                new Address("300 Walk St", 300, Direction.None, "Walk", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                false,
                false,
                false,
                false
            };
        }

        public static IEnumerable<object[]> ReversalAddress() {
            yield return new object[] {
                "1625 SOUTH 672 EAST",
                "672 east 1625 south"
            };
        }

        public static IEnumerable<object[]> OneSuffixDirectionLikeApartment() {
            yield return new object[] {
                new Address("582 DAMMERON VALLEY DR W", 582, Direction.None, "DAMMERON VALLEY", StreetType.Drive, Direction.West, null, 0, null, 0, false, false),
                "582 dammeron valley drive west"
                    };
        }

        public static IEnumerable<object[]> PeriodInAddress() {
            yield return new object[] {
                new Address("326 east south temple st.", 326, Direction.East, "south temple", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "326 east south temple street"
            };
        }

        public static IEnumerable<object[]> SillyAbbreviations() {
            yield return new object[] {
                new Address("9258 So 3090 W", 9258, Direction.South, "3090", StreetType.None, Direction.West, null, 0, null, 0, false, false),
                "9258 south 3090 west"
            };

            yield return new object[] {
                new Address("4536 W 6090 So", 4536, Direction.West, "6090", StreetType.None, Direction.South, null, 0, null, 0, false, false),
                "4536 west 6090 south"
            };

            yield return new object[] {
                new Address("4536 W. 6090 So.", 4536, Direction.West, "6090", StreetType.None, Direction.South, null, 0, null, 0, false, false),
                "4536 west 6090 south"
            };
        }

        public static IEnumerable<object[]> CommonMisspelling() {
            yield return new object[] {
                new Address("5974 FASHION POINT DR", 5974, Direction.None, "FASHION POINT", StreetType.Drive, Direction.None, null, 0, null, 0, false, false),
                "5974 fashion point drive"
            };

            yield return new object[] {
                new Address("1551 S RENAISSANCE TWN DR SUITE 420", 1551, Direction.South, "RENAISSANCE TWN", StreetType.Drive, Direction.None, null, 0, null, 0, false, false),
                "1551 south renaissance twn drive"
            };
        }

        public static IEnumerable<object[]> GithubIssues() {
            yield return new object[] {
                new Address("9211 N Pebble Creek Loop", 9211, Direction.North, "Pebble Creek", StreetType.Loop, Direction.None, null, 0, null, 0, false, false),
                "9211 north pebble creek loop"
            };

            yield return new object[] {
                new Address("180 N STATE ST", 180, Direction.North, "STATE", StreetType.Street, Direction.None, null, 0, null, 0, false, false),
                "180 north state street"
            };

            // yield return new object[] {
            //     new Address("455 E APPLE BLOSSOM LN E",
            //         HouseNumber = 455,
            //         PrefixDirection = Direction.East,
            //         StreetName = "APPLE BLOSSOM",
            //         SuffixDirection = Direction.East,
            //         StreetType = StreetType.Lane
            //     },
            //     "455 east apple blossom lane east"
            // };
        }

        [Theory]
        [MemberData(nameof(GetPoBoxes))]
        public async Task Should_parse_input_with_spaces(Address input, string standardAddress, bool reversal) {
            var request = new AddressParsing.Computation(input.InputAddress);
            var result = await _handler.Handle(request, CancellationToken.None);

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

        [Theory]
        [MemberData(nameof(UnitsWithNoTypeDesignation))]
        [MemberData(nameof(AbbreviatedHighways))]
        [MemberData(nameof(SecondaryUnits))]
        [MemberData(nameof(EnumsInStreetName))]
        [MemberData(nameof(Concatenations))]
        [MemberData(nameof(NonStandardNumericStreetNames))]
        [MemberData(nameof(OneCharacterStreetNameOrHouseNumber))]
        [MemberData(nameof(OrdinalStreetNames))]
        [MemberData(nameof(OneSuffixDirectionLikeApartment))]
        [MemberData(nameof(PeriodInAddress))]
        [MemberData(nameof(SillyAbbreviations))]
        [MemberData(nameof(CommonMisspelling))]
        [MemberData(nameof(GithubIssues))]
        public async Task Should_parse_address_parts(Address input, string standardAddress) {
            var request = new AddressParsing.Computation(input.InputAddress);
            var result = await _handler.Handle(request, CancellationToken.None);

            result.PoBox.ShouldBe(0);
            result.HouseNumber.ShouldBe(input.HouseNumber);
            result.PrefixDirection.ShouldBe(input.PrefixDirection);
            result.StreetName.ShouldBe(input.StreetName);
            result.StreetType.ShouldBe(input.StreetType);
            result.SuffixDirection.ShouldBe(input.SuffixDirection);
            result.StandardizedAddress.ToLowerInvariant().ShouldBe(standardAddress);
            result.IsPoBox.ShouldBeFalse();
        }

        [Theory]
        [MemberData(nameof(OriginalTestAddresses))]
        public async Task Should_parse_address_parts_with_insights(Address input, bool reversal,
                                                                   bool possibleReversal, bool hasPrefix,
                                                                   bool isNumeric) {
            var request = new AddressParsing.Computation(input.InputAddress);
            var result = await _handler.Handle(request, CancellationToken.None);

            result.HouseNumber.ShouldBe(input.HouseNumber);
            result.PrefixDirection.ShouldBe(input.PrefixDirection);
            result.StreetName.ShouldBe(input.StreetName);
            result.StreetType.ShouldBe(input.StreetType);
            result.SuffixDirection.ShouldBe(input.SuffixDirection);
            result.PoBox.ShouldBe(0);
            result.IsPoBox.ShouldBeFalse();
            result.IsHighway.ShouldBeFalse();
            result.IsIntersection().ShouldBeFalse();
            result.IsNumericStreetName().ShouldBe(isNumeric);
            result.HasPrefix().ShouldBe(hasPrefix);
            result.IsReversal().ShouldBe(reversal);
            result.PossibleReversal().ShouldBe(possibleReversal);
        }

        [Theory]
        [MemberData(nameof(ReversalAddress))]
        public async Task Should_create_reversal_address(string address, string reversalAddress) {
            var request = new AddressParsing.Computation(address);
            var result = await _handler.Handle(request, CancellationToken.None);

            result.ReversalAddress.ToLowerInvariant().ShouldBe(reversalAddress);
        }

        [Fact]
        public async Task Should_not_flag_house_address_as_pobox() {
            var request = new AddressParsing.Computation("123 west house st");
            var result = await _handler.Handle(request, CancellationToken.None);

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
