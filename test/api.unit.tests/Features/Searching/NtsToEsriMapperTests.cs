using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Features.Searching;
using NetTopologySuite.Geometries;
using Shouldly;
using Xunit;
using static AGRC.api.Features.Converting.EsriGraphic;

namespace api.tests.Features.Searching {
    public class NtsToEsriMapperTests {
        [Fact]
        public async Task Should_convert_polygon_to_graphic() {
            var geometry = new Polygon(
                new LinearRing(new[] {
                    new Coordinate(0, 0),
                    new Coordinate(1, 0),
                    new Coordinate(1, 1),
                    new Coordinate(0, 1),
                    new Coordinate(0, 0),
                })
            );

            var computation = new NtsToEsriMapper.Computation(geometry);

            var handler = new NtsToEsriMapper.Handler();
            var result = await handler.Handle(computation, CancellationToken.None);

            result.ShouldBeOfType<SerializableGraphic>();
            result.Geometry.ShouldBeAssignableTo<EsriJson.Net.Geometry.Polygon>();
            var polygon = result.Geometry as EsriJson.Net.Geometry.Polygon;

            polygon.Rings.Count.ShouldBe(1);
            polygon.Rings[0].Length.ShouldBe(5);

            var ring = polygon.Rings[0];

            ring[0].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);
            ring[1].Equals(new EsriJson.Net.Geometry.RingPoint(1, 0)).ShouldBe(true);
            ring[2].Equals(new EsriJson.Net.Geometry.RingPoint(1, 1)).ShouldBe(true);
            ring[3].Equals(new EsriJson.Net.Geometry.RingPoint(0, 1)).ShouldBe(true);
            ring[4].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);
        }

        [Fact]
        public async Task Should_convert_polygon_with_holes_to_graphic() {
            var geometry = new Polygon(
                new LinearRing(new[] {
                    new Coordinate(0, 0),
                    new Coordinate(1, 0),
                    new Coordinate(1, 1),
                    new Coordinate(0, 1),
                    new Coordinate(0, 0),
                }), new[] {
                    new LinearRing(new [] {
                        new Coordinate(.5,.5),
                        new Coordinate(.6, .6),
                        new Coordinate(.4,.6),
                        new Coordinate(.5, .5)
                    })
                }
            );

            var computation = new NtsToEsriMapper.Computation(geometry);

            var handler = new NtsToEsriMapper.Handler();
            var result = await handler.Handle(computation, CancellationToken.None);

            result.ShouldBeOfType<SerializableGraphic>();
            result.Geometry.ShouldBeAssignableTo<EsriJson.Net.Geometry.Polygon>();
            var polygon = result.Geometry as EsriJson.Net.Geometry.Polygon;

            polygon.Rings.Count.ShouldBe(2);
            polygon.Rings[0].Length.ShouldBe(5);
            polygon.Rings[1].Length.ShouldBe(4);

            var ring = polygon.Rings[0];

            ring[0].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);
            ring[1].Equals(new EsriJson.Net.Geometry.RingPoint(1, 0)).ShouldBe(true);
            ring[2].Equals(new EsriJson.Net.Geometry.RingPoint(1, 1)).ShouldBe(true);
            ring[3].Equals(new EsriJson.Net.Geometry.RingPoint(0, 1)).ShouldBe(true);
            ring[4].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);

            var ring2 = polygon.Rings[1];

            ring2[0].Equals(new EsriJson.Net.Geometry.RingPoint(.5, .5)).ShouldBe(true);
            ring2[1].Equals(new EsriJson.Net.Geometry.RingPoint(.6, .6)).ShouldBe(true);
            ring2[2].Equals(new EsriJson.Net.Geometry.RingPoint(.4, .6)).ShouldBe(true);
            ring2[3].Equals(new EsriJson.Net.Geometry.RingPoint(.5, .5)).ShouldBe(true);
        }

        [Fact]
        public async Task Should_convert_multipolygon_to_graphic() {
            var geometry = new MultiPolygon(
                new[] {new Polygon(new LinearRing(new[] {
                    new Coordinate(0, 0),
                    new Coordinate(1, 0),
                    new Coordinate(1, 1),
                    new Coordinate(0, 1),
                    new Coordinate(0, 0),
                })), new Polygon(new LinearRing(new [] {
                    new Coordinate(.5,.5),
                    new Coordinate(.6, .6),
                    new Coordinate(.4,.6),
                    new Coordinate(.5, .5)
                }))
                }
            );

            var computation = new NtsToEsriMapper.Computation(geometry);

            var handler = new NtsToEsriMapper.Handler();
            var result = await handler.Handle(computation, CancellationToken.None);

            result.ShouldBeOfType<SerializableGraphic>();
            result.Geometry.ShouldBeAssignableTo<EsriJson.Net.Geometry.Polygon>();
            var polygon = result.Geometry as EsriJson.Net.Geometry.Polygon;

            polygon.Rings.Count.ShouldBe(2);
            polygon.Rings[0].Length.ShouldBe(5);
            polygon.Rings[1].Length.ShouldBe(4);

            var ring = polygon.Rings[0];

            ring[0].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);
            ring[1].Equals(new EsriJson.Net.Geometry.RingPoint(1, 0)).ShouldBe(true);
            ring[2].Equals(new EsriJson.Net.Geometry.RingPoint(1, 1)).ShouldBe(true);
            ring[3].Equals(new EsriJson.Net.Geometry.RingPoint(0, 1)).ShouldBe(true);
            ring[4].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);

            var ring2 = polygon.Rings[1];

            ring2[0].Equals(new EsriJson.Net.Geometry.RingPoint(.5, .5)).ShouldBe(true);
            ring2[1].Equals(new EsriJson.Net.Geometry.RingPoint(.6, .6)).ShouldBe(true);
            ring2[2].Equals(new EsriJson.Net.Geometry.RingPoint(.4, .6)).ShouldBe(true);
            ring2[3].Equals(new EsriJson.Net.Geometry.RingPoint(.5, .5)).ShouldBe(true);
        }
    }
}
