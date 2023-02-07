using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class AddressParsingTests {
        public AddressParsingTests() {
            var abbreviations = new Abbreviations();
            var regex = new RegexCache(abbreviations);

            var mock = new Mock<ILogger> { DefaultValue = DefaultValue.Mock };

            _handler = new AddressParsing.Handler(regex, abbreviations, mock.Object);
        }

        private readonly IComputationHandler<AddressParsing.Computation, CleansedAddress> _handler;

        public static IEnumerable<object[]> GetPoBoxes() {
            yield return new object[] {
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

            yield return new object[] {
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

            yield return new object[] {
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

            yield return new object[] {
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

        public static IEnumerable<object[]> UnitsWithNoTypeDesignation() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "625 NORTH REDWOOD ROAD 6",
                    HouseNumber = 625,
                    PrefixDirection = Direction.North,
                    StreetName = "REDWOOD",
                    StreetType = StreetType.Road,
                    SuffixDirection = Direction.None
                },
                "625 north redwood road"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "295 North 120 West 1",
                    HouseNumber = 295,
                    PrefixDirection = Direction.North,
                    StreetName = "120",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.West
                },
                "295 north 120 west"
            };
        }

        public static IEnumerable<object[]> AbbreviatedHighways() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "401 N HWY 68",
                    HouseNumber = 401,
                    PrefixDirection = Direction.North,
                    StreetName = "Highway 68",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.None
                },
                "401 north highway 68"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "401 N SR 68",
                    HouseNumber = 401,
                    PrefixDirection = Direction.North,
                    StreetName = "Highway 68",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.None
                },
                "401 north highway 68"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "401 N US 89",
                    HouseNumber = 401,
                    PrefixDirection = Direction.North,
                    StreetName = "Highway 89",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.None
                },
                "401 north highway 89"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "401 N U.S. 89",
                    HouseNumber = 401,
                    PrefixDirection = Direction.North,
                    StreetName = "Highway 89",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.None
                },
                "401 north highway 89"
            };
        }

        public static IEnumerable<object[]> EnumsInStreetName() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "4463 SUMMER PLACE CIR",
                    HouseNumber = 4463,
                    PrefixDirection = Direction.None,
                    StreetName = "SUMMER PLACE",
                    StreetType = StreetType.Circle,
                    SuffixDirection = Direction.None
                },
                "4463 summer place circle"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "4463 SUMMER PL CIR",
                    HouseNumber = 4463,
                    PrefixDirection = Direction.None,
                    StreetName = "SUMMER PL",
                    StreetType = StreetType.Circle,
                    SuffixDirection = Direction.None
                },
                "4463 summer pl circle"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "478 S WEST FRONTAGE RD",
                    HouseNumber = 478,
                    PrefixDirection = Direction.South,
                    StreetName = "WEST FRONTAGE",
                    StreetType = StreetType.Road,
                    SuffixDirection = Direction.None
                },
                "478 south west frontage road"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "5301 w jacob hill cir",
                    HouseNumber = 5301,
                    PrefixDirection = Direction.West,
                    StreetName = "jacob hill",
                    StreetType = StreetType.Circle,
                    SuffixDirection = Direction.None
                },
                "5301 west jacob hill circle"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "5811 W Park West Rd",
                    HouseNumber = 5811,
                    PrefixDirection = Direction.West,
                    StreetName = "Park West",
                    SuffixDirection = Direction.None,
                    StreetType = StreetType.Road
                },
                "5811 west park west road"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "6112 W Whistle Stop Road",
                    HouseNumber = 6112,
                    PrefixDirection = Direction.West,
                    StreetName = "Whistle Stop",
                    SuffixDirection = Direction.None,
                    StreetType = StreetType.Road
                },
                "6112 west whistle stop road"
            };
        }

        public static IEnumerable<object[]> SecondaryUnits() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "435 N MAIN ST bsmt",
                    HouseNumber = 435,
                    PrefixDirection = Direction.North,
                    StreetName = "MAIN",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                "435 north main street"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "435 N MAIN ST PENTHOUSE",
                    HouseNumber = 435,
                    PrefixDirection = Direction.North,
                    StreetName = "MAIN",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                "435 north main street"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "436 N MAIN ST APT 7",
                    HouseNumber = 436,
                    PrefixDirection = Direction.North,
                    StreetName = "MAIN",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                "436 north main street"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "3700 W 150 N LOT 122",
                    HouseNumber = 3700,
                    PrefixDirection = Direction.West,
                    StreetName = "150",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.North
                },
                "3700 west 150 north"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "222 N 1200 W TRLR 44",
                    HouseNumber = 222,
                    PrefixDirection = Direction.North,
                    StreetName = "1200",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.West
                },
                "222 north 1200 west"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1015 S RIVER RD UNIT 42",
                    HouseNumber = 1015,
                    PrefixDirection = Direction.South,
                    StreetName = "RIVER",
                    StreetType = StreetType.Road,
                    SuffixDirection = Direction.None
                },
                "1015 south river road"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "859 PINEWAY DR APT 11E",
                    HouseNumber = 859,
                    PrefixDirection = Direction.None,
                    StreetName = "PINEWAY",
                    StreetType = StreetType.Drive,
                    SuffixDirection = Direction.None
                },
                "859 pineway drive"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "902 E 500 S APT F",
                    HouseNumber = 902,
                    PrefixDirection = Direction.East,
                    StreetName = "500",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.South
                },
                "902 east 500 south"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1388 W 699 S Bsmt",
                    HouseNumber = 1388,
                    PrefixDirection = Direction.West,
                    StreetName = "699",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.South
                },
                "1388 west 699 south"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1389 W 699 S Suite 900",
                    HouseNumber = 1389,
                    PrefixDirection = Direction.West,
                    StreetName = "699",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.South
                },
                "1389 west 699 south"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1490 E FOREMASTER DR STE 150",
                    HouseNumber = 1490,
                    PrefixDirection = Direction.East,
                    StreetName = "FOREMASTER",
                    SuffixDirection = Direction.None,
                    StreetType = StreetType.Drive
                },
                "1490 east foremaster drive"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1387 W 699 S Ste 900",
                    HouseNumber = 1387,
                    PrefixDirection = Direction.West,
                    StreetName = "699",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.South
                },
                "1387 west 699 south"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1387 W 699 S Ste B150",
                    HouseNumber = 1387,
                    PrefixDirection = Direction.West,
                    StreetName = "699",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.South
                },
                "1387 west 699 south"
            };
        }

        public static IEnumerable<object[]> Concatenations() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1991S 800E",
                    HouseNumber = 1991,
                    PrefixDirection = Direction.South,
                    StreetName = "800",
                    SuffixDirection = Direction.East
                },
                "1991 south 800 east"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1991S 900 E",
                    HouseNumber = 1991,
                    PrefixDirection = Direction.South,
                    StreetName = "900",
                    SuffixDirection = Direction.East
                },
                "1991 south 900 east"
            };
        }

        public static IEnumerable<object[]> NonStandardNumericStreetNames() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1048 W 1205 N",
                    HouseNumber = 1048,
                    PrefixDirection = Direction.West,
                    StreetName = "1205",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.North
                },
                "1048 west 1205 north"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "2139 N 50 W",
                    HouseNumber = 2139,
                    PrefixDirection = Direction.North,
                    StreetName = "50",
                    StreetType = StreetType.None,
                    SuffixDirection = Direction.West
                },
                "2139 north 50 west"
            };
        }

        public static IEnumerable<object[]> OneCharacterStreetNameOrHouseNumber() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "168 N ST",
                    HouseNumber = 168,
                    PrefixDirection = Direction.None,
                    StreetName = "N",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                "168 n street"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "168 N N ST",
                    HouseNumber = 168,
                    PrefixDirection = Direction.North,
                    StreetName = "N",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                "168 north n street"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "5 Cedar Ave",
                    HouseNumber = 5,
                    PrefixDirection = Direction.None,
                    StreetName = "Cedar",
                    StreetType = StreetType.Avenue,
                    SuffixDirection = Direction.None
                },
                "5 cedar avenue"
            };
        }

        public static IEnumerable<object[]> OrdinalStreetNames() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1238 E 1ST Avenue",
                    HouseNumber = 1238,
                    PrefixDirection = Direction.East,
                    StreetName = "1ST",
                    StreetType = StreetType.Avenue,
                    SuffixDirection = Direction.None
                },
                "1238 east 1st avenue"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1238 E FIRST Avenue",
                    HouseNumber = 1238,
                    PrefixDirection = Direction.East,
                    StreetName = "FIRST",
                    StreetType = StreetType.Avenue,
                    SuffixDirection = Direction.None
                },
                "1238 east first avenue"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1238 E 2ND Avenue",
                    HouseNumber = 1238,
                    PrefixDirection = Direction.East,
                    StreetName = "2ND",
                    StreetType = StreetType.Avenue,
                    SuffixDirection = Direction.None
                },
                "1238 east 2nd avenue"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1238 E 3RD Avenue",
                    HouseNumber = 1238,
                    PrefixDirection = Direction.East,
                    StreetName = "3RD",
                    StreetType = StreetType.Avenue,
                    SuffixDirection = Direction.None
                },
                "1238 east 3rd avenue"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1573 24TH Street",
                    HouseNumber = 1573,
                    PrefixDirection = Direction.None,
                    StreetName = "24TH",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                "1573 24th street"
            };
        }

        public static IEnumerable<object[]> OriginalTestAddresses() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "400 S 532 E",
                    HouseNumber = 400,
                    PrefixDirection = Direction.South,
                    StreetName = "532",
                    SuffixDirection = Direction.East
                },
                true,
                false,
                true,
                true
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "5625 S 995 E",
                    HouseNumber = 5625,
                    PrefixDirection = Direction.South,
                    StreetName = "995",
                    SuffixDirection = Direction.East
                },
                false,
                true,
                true,
                true
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "372 North 600 East",
                    HouseNumber = 372,
                    PrefixDirection = Direction.North,
                    StreetName = "600",
                    SuffixDirection = Direction.East
                },
                false,
                false,
                true,
                true
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "30 WEST 300 NORTH",
                    HouseNumber = 30,
                    PrefixDirection = Direction.West,
                    StreetName = "300",
                    SuffixDirection = Direction.North
                },
                false,
                true,
                true,
                true
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "126 E 400 N",
                    HouseNumber = 126,
                    PrefixDirection = Direction.East,
                    StreetName = "400",
                    SuffixDirection = Direction.North
                },
                false,
                false,
                true,
                true
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "270 South 1300 East",
                    HouseNumber = 270,
                    PrefixDirection = Direction.South,
                    StreetName = "1300",
                    SuffixDirection = Direction.East
                },
                false,
                true,
                true,
                true
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "126 W SEGO LILY DR",
                    HouseNumber = 126,
                    PrefixDirection = Direction.West,
                    StreetName = "SEGO LILY",
                    StreetType = StreetType.Drive,
                    SuffixDirection = Direction.None
                },
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "261 E MUELLER PARK RD",
                    HouseNumber = 261,
                    PrefixDirection = Direction.East,
                    StreetName = "MUELLER PARK",
                    StreetType = StreetType.Road,
                    SuffixDirection = Direction.None
                },
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "17 S VENICE MAIN ST",
                    HouseNumber = 17,
                    PrefixDirection = Direction.South,
                    StreetName = "VENICE MAIN",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "20 W Center St",
                    HouseNumber = 20,
                    PrefixDirection = Direction.West,
                    StreetName = "Center",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                false,
                false,
                true,
                false
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "9314 ALVEY LN",
                    HouseNumber = 9314,
                    PrefixDirection = Direction.None,
                    StreetName = "ALVEY",
                    StreetType = StreetType.Lane,
                    SuffixDirection = Direction.None
                },
                false,
                false,
                false,
                false
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "167 DALY AVE",
                    HouseNumber = 167,
                    PrefixDirection = Direction.None,
                    StreetName = "DALY",
                    StreetType = StreetType.Avenue,
                    SuffixDirection = Direction.None
                },
                false,
                false,
                false,
                false
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1147 MCDANIEL CIR",
                    HouseNumber = 1147,
                    PrefixDirection = Direction.None,
                    StreetName = "MCDANIEL",
                    StreetType = StreetType.Circle,
                    SuffixDirection = Direction.None
                },
                false,
                false,
                false,
                false
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "300 Walk St",
                    HouseNumber = 300,
                    PrefixDirection = Direction.None,
                    StreetName = "Walk",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
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
                new CleansedAddress {
                    InputAddress = "582 DAMMERON VALLEY DR W",
                    HouseNumber = 582,
                    PrefixDirection = Direction.None,
                    StreetName = "DAMMERON VALLEY",
                    SuffixDirection = Direction.West,
                    StreetType = StreetType.Drive
                },
                "582 dammeron valley drive west"
            };
        }

        public static IEnumerable<object[]> PeriodInAddress() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "326 east south temple st.",
                    HouseNumber = 326,
                    PrefixDirection = Direction.East,
                    StreetName = "south temple",
                    SuffixDirection = Direction.None,
                    StreetType = StreetType.Street
                },
                "326 east south temple street"
            };
        }

        public static IEnumerable<object[]> SillyAbbreviations() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "9258 So 3090 W",
                    HouseNumber = 9258,
                    PrefixDirection = Direction.South,
                    StreetName = "3090",
                    SuffixDirection = Direction.West,
                    StreetType = StreetType.None
                },
                "9258 south 3090 west"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "4536 W 6090 So",
                    HouseNumber = 4536,
                    PrefixDirection = Direction.West,
                    StreetName = "6090",
                    SuffixDirection = Direction.South,
                    StreetType = StreetType.None
                },
                "4536 west 6090 south"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "4536 W. 6090 So.",
                    HouseNumber = 4536,
                    PrefixDirection = Direction.West,
                    StreetName = "6090",
                    SuffixDirection = Direction.South,
                    StreetType = StreetType.None
                },
                "4536 west 6090 south"
            };
        }

        public static IEnumerable<object[]> CommonMisspelling() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "5974 FASHION POINT DR",
                    HouseNumber = 5974,
                    PrefixDirection = Direction.None,
                    StreetName = "FASHION POINT",
                    SuffixDirection = Direction.None,
                    StreetType = StreetType.Drive
                },
                "5974 fashion point drive"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "1551 S RENAISSANCE TWN DR SUITE 420",
                    HouseNumber = 1551,
                    PrefixDirection = Direction.South,
                    StreetName = "RENAISSANCE TWN",
                    SuffixDirection = Direction.None,
                    StreetType = StreetType.Drive
                },
                "1551 south renaissance twn drive"
            };
        }

        public static IEnumerable<object[]> GithubIssues() {
            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "9211 N Pebble Creek Loop",
                    HouseNumber = 9211,
                    PrefixDirection = Direction.North,
                    StreetName = "Pebble Creek",
                    SuffixDirection = Direction.None,
                    StreetType = StreetType.Loop
                },
                "9211 north pebble creek loop"
            };

            yield return new object[] {
                new CleansedAddress {
                    InputAddress = "180 N STATE ST",
                    HouseNumber = 180,
                    PrefixDirection = Direction.North,
                    StreetName = "STATE",
                    StreetType = StreetType.Street,
                    SuffixDirection = Direction.None
                },
                "180 north state street"
            };

            // yield return new object[] {
            //     new CleansedAddress {
            //         InputAddress = "455 E APPLE BLOSSOM LN E",
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
        public async Task Should_parse_input_with_spaces(CleansedAddress input, string standardAddress, bool reversal) {
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
        public async Task Should_parse_address_parts(CleansedAddress input, string standardAddress) {
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
        public async Task Should_parse_address_parts_with_insights(CleansedAddress input, bool reversal,
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
