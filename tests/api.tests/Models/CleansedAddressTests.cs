using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.Constants;
using Shouldly;
using Xunit;

namespace api.tests.Models {
    public class CleansedAddressTests {
        [Fact]
        public void Should_create_reversal() {
            var model = new CleansedAddress {
                HouseNumber = 1,
                PrefixDirection = Direction.West,
                StreetName = "street name",
                SuffixDirection = Direction.South,
                StreetType = StreetType.Alley
            };

            model.ReversalAddress.ToLowerInvariant().ShouldBe("street name south alley 1 west");
        }

        [Theory]
        [InlineData("123 south main st", false)]
        [InlineData("123 band sand south andy", false)]
        [InlineData("123 south main and 326 south temple", true)]
        public void Should_know_if_intersection(string address, bool isIntersection) {
            var model = new CleansedAddress {
                InputAddress = address
            };

            model.IsIntersection().ShouldBe(isIntersection);
        }

        [Theory]
        [InlineData(700, "230", true)]
        [InlineData(15, "15", true)]
        [InlineData(15, "street", false)]
        [InlineData(5, "south main", false)]
        [InlineData(5, "", false)]
        public void Should_know_if_could_be_reversal(int? number, string name, bool maybeReversal) {
            var model = new CleansedAddress {
                StreetName = name,
                HouseNumber = number
            };

            model.PossibleReversal().ShouldBe(maybeReversal);
        }

        [Theory]
        [InlineData(5, "101", true)]
        [InlineData(10, "102", true)]
        [InlineData(15, "103", true)]
        [InlineData(20, "104", true)]
        [InlineData(25, "106", true)]
        [InlineData(30, "107", true)]
        [InlineData(35, "108", true)]
        [InlineData(40, "109", true)]
        [InlineData(45, "street", false)]
        [InlineData(50, "south main", false)]
        [InlineData(55, "", false)]
        public void Should_know_if_reversal(int? number, string name, bool reversal) {
            var model = new CleansedAddress {
                StreetName = name,
                HouseNumber = number
            };

            model.IsReversal().ShouldBe(reversal);
        }

        [Fact]
        public void Should_standardize_address() {
            var model = new CleansedAddress {
                HouseNumber = 1,
                PrefixDirection = Direction.East,
                StreetName = "street",
                StreetType = StreetType.Alley,
                SuffixDirection = Direction.North
            };

            model.StandardizedAddress.ToLowerInvariant().ShouldBe("1 east street alley north");
        }

        [Fact]
        public void Should_standardize_address_without_nones() {
            var model = new CleansedAddress {
                HouseNumber = 1,
                PrefixDirection = Direction.None,
                StreetName = "street",
                StreetType = StreetType.None,
                SuffixDirection = Direction.None
            };

            model.StandardizedAddress.ToLowerInvariant().ShouldBe("1 street");
        }

        [Fact]
        public void Should_standardize_po_boxes() {
            var model = new CleansedAddress {
                PoBox = 1,
                IsPoBox = true
            };

            model.StandardizedAddress.ToLowerInvariant().ShouldBe("p.o. box 1");
        }
    }
}
