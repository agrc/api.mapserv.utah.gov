using System;
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
        internal static IRequestHandler<AddressSystemFromPlace.Command, IReadOnlyCollection<GridLinkable>> handler;
        internal static readonly CancellationToken cancellation;

        private readonly Dictionary<string, List<GridLinkable>> _links = new Dictionary<string, List<GridLinkable>>(1);

        public AddressSystemFromPlaceTests()
        {
            _links.Add("place", new List<GridLinkable> { new PlaceGridLink("place", "grid", 1) });
            var mockCache = new Mock<ILookupCache>();
            mockCache.Setup(x => x.PlaceGrids).Returns(_links);

            handler = new AddressSystemFromPlace.Handler(mockCache.Object);
        }

        [Fact]
        public async Task Should_return_grid_from_place() {
            var request = new AddressSystemFromPlace.Command("place");
            var result = await handler.Handle(request, cancellation);

            result.Count.ShouldBe(1);
            result.First().Grid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_empty_when_zip_not_found() {
            var request = new AddressSystemFromPlace.Command("other place");
            var result = await handler.Handle(request, cancellation);

            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_return_empty_when_zip_is_null() {
            var place = string.Empty;
            var request = new AddressSystemFromPlace.Command(place);
            var result = await handler.Handle(request, cancellation);

            result.ShouldBeEmpty();
        }
    }
}
