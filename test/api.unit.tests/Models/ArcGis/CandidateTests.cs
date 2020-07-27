using AGRC.api.Models.ArcGis;
using Shouldly;
using Xunit;

namespace api.tests.Models.ArcGis {
    public class CandidateTests {
        [Fact]
        public void Should_handle_empty_address() {
            const string address = "";
            var candidate = new Candidate {
                Address = address
            };

            candidate.AddressGrid.ShouldBeNull();
            candidate.Address.ShouldBeEmpty();
        }

        [Fact]
        public void Should_set_address() {
            const string address = "123 main street, grid";
            var candidate = new Candidate {
                Address = address
            };

            candidate.AddressGrid.ShouldBeNull();
            candidate.Address.ShouldBe(address);
        }

        // I'm not sure why the code does this
        [Fact]
        public void Should_split_address_grid_from_address() {
            const string address = "123 main street, grid, utah";
            var candidate = new Candidate {
                Address = address
            };

            candidate.AddressGrid.ShouldBe("Grid");
            candidate.Address.ShouldBe("123 main street, utah");
        }
    }
}
