using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Cache;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models.Linkables;
using MediatR;
using Moq;
using Shouldly;
using Xunit;

namespace api.tests.Features.Geocoding {
    public class AddressSystemFromPlaceTests {
        public AddressSystemFromPlaceTests() {
            _links.Add("place", new List<GridLinkable> {new PlaceGridLink("place", "grid", 1)});
            var mockCache = new Mock<ILookupCache>();
            mockCache.Setup(x => x.PlaceGrids).Returns(_links);

            Handler = new AddressSystemFromPlace.Handler(mockCache.Object);
        }

        internal static IRequestHandler<AddressSystemFromPlace.Command, IReadOnlyCollection<GridLinkable>> Handler;

        private readonly Dictionary<string, List<GridLinkable>> _links = new Dictionary<string, List<GridLinkable>>(1);

        [Fact]
        public async Task Should_return_empty_when_zip_is_null() {
            var place = string.Empty;
            var request = new AddressSystemFromPlace.Command(place);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_return_empty_when_zip_not_found() {
            var request = new AddressSystemFromPlace.Command("other place");
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_return_grid_from_place() {
            var request = new AddressSystemFromPlace.Command("place");
            var result = await Handler.Handle(request, CancellationToken.None);

            result.Count.ShouldBe(1);
            result.First().Grid.ShouldBe("grid");
        }
    }
}
