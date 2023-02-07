using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Features.Geocoding;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Linkables;
using Moq;
using Serilog;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class AddressSystemFromPlaceTests {
        public AddressSystemFromPlaceTests() {
            _links.Add("place", new List<GridLinkable> { new PlaceGridLink("place", "grid", 1) });
            var mockCache = new Mock<ILookupCache>();
            mockCache.Setup(x => x.PlaceGrids).Returns(_links);

            var mock = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

            Handler = new AddressSystemFromPlace.Handler(mockCache.Object, mock.Object);
        }

        internal static IComputationHandler<AddressSystemFromPlace.Computation, IReadOnlyCollection<GridLinkable>> Handler;

        private readonly Dictionary<string, List<GridLinkable>> _links = new(1);

        [Fact]
        public async Task Should_return_empty_when_zip_is_null() {
            var place = string.Empty;
            var request = new AddressSystemFromPlace.Computation(place);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_return_empty_when_zip_not_found() {
            var request = new AddressSystemFromPlace.Computation("other place");
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_return_grid_from_place() {
            var request = new AddressSystemFromPlace.Computation("place");
            var result = await Handler.Handle(request, CancellationToken.None);

            result.Count.ShouldBe(1);
            result.First().Grid.ShouldBe("grid");
        }
    }
}
