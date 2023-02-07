using System;
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
    public class AddressSystemFromZipTests {
        internal static IComputationHandler<AddressSystemFromZipCode.Computation, IReadOnlyCollection<GridLinkable>> Handler;

        private readonly Dictionary<string, List<GridLinkable>> _links = new(1);

        public AddressSystemFromZipTests() {
            _links.Add("1", new List<GridLinkable> { new ZipGridLink(1, "grid", 1) });
            var mockCache = new Mock<ILookupCache>();
            mockCache.Setup(x => x.ZipCodesGrids).Returns(_links);

            var mock = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock };

            Handler = new AddressSystemFromZipCode.Handler(mockCache.Object, mock.Object);
        }

        [Fact]
        public async Task Should_return_grid_from_zip() {
            var request = new AddressSystemFromZipCode.Computation(1);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.Count.ShouldBe(1);
            result.First().Grid.ShouldBe("grid");
        }

        [Fact]
        public async Task Should_return_empty_when_zip_not_found() {
            var request = new AddressSystemFromZipCode.Computation(0);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task Should_return_empty_when_zip_is_null() {
            var request = new AddressSystemFromZipCode.Computation(null);
            var result = await Handler.Handle(request, CancellationToken.None);

            result.ShouldBeEmpty();
        }
    }
}
