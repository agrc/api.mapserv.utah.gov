using ugrc.api.Features.Searching;
using NetTopologySuite.Geometries;

namespace api.tests.Features.Searching;
public class NtsToEsriMapperTests {
    public ILogger Logger { get; set; }
    public NtsToEsriMapperTests() {
        Logger = new Mock<ILogger>() { DefaultValue = DefaultValue.Mock }.Object;
    }

    [Fact]
    public async Task Should_convert_point_to_esri_geometry() {
        var geometry = new Point(new Coordinate(1, 0));

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.Point>();
        var resultGeometry = result as EsriJson.Net.Geometry.Point;

        resultGeometry.X.ShouldBe(1d);
        resultGeometry.Y.ShouldBe(0d);
        resultGeometry.CRS.WellKnownId.ShouldBe(26912);
    }

    [Fact]
    public async Task Should_convert_point_with_srid_to_esri_geometry() {
        var geometry = new Point(new Coordinate(1, 0)) {
            SRID = 3857
        };

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.Point>();
        var resultGeometry = result as EsriJson.Net.Geometry.Point;

        resultGeometry.X.ShouldBe(1d);
        resultGeometry.Y.ShouldBe(0d);
        resultGeometry.CRS.WellKnownId.ShouldBe(3857);
    }

    [Fact]
    public async Task Should_convert_multipoint_to_esri_geometry() {
        var geometry = new MultiPoint([
            new Point(new Coordinate(1, 0)),
            new Point(new Coordinate(11, 10))
        ]);

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.MultiPoint>();
        var resultGeometry = result as EsriJson.Net.Geometry.MultiPoint;

        resultGeometry.Points.Length.ShouldBe(2);

        resultGeometry.Points[0].X.ShouldBe(1d);
        resultGeometry.Points[0].Y.ShouldBe(0d);
        resultGeometry.Points[1].X.ShouldBe(11d);
        resultGeometry.Points[1].Y.ShouldBe(10d);
    }

    [Fact]
    public async Task Should_convert_polyline_to_esri_geometry() {
        var geometry = new LineString(
            [
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0),
            ]
        );

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.Polyline>();
        var resultGeometry = result as EsriJson.Net.Geometry.Polyline;

        resultGeometry.Paths.Count.ShouldBe(1);
        resultGeometry.Paths[0].Length.ShouldBe(5);

        var ring = resultGeometry.Paths[0];

        ring[0].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);
        ring[1].Equals(new EsriJson.Net.Geometry.RingPoint(1, 0)).ShouldBe(true);
        ring[2].Equals(new EsriJson.Net.Geometry.RingPoint(1, 1)).ShouldBe(true);
        ring[3].Equals(new EsriJson.Net.Geometry.RingPoint(0, 1)).ShouldBe(true);
        ring[4].Equals(new EsriJson.Net.Geometry.RingPoint(0, 0)).ShouldBe(true);
    }

    [Fact]
    public async Task Should_convert_multiline_to_esri_geometry() {
        var geometry = new MultiLineString([
            new LineString([
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0),
            ]),
            new LineString([
                new Coordinate(10, 10),
                new Coordinate(11, 10),
                new Coordinate(11, 11),
                new Coordinate(10, 11),
            ]),
        ]);

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.Polyline>();
        var resultGeometry = result as EsriJson.Net.Geometry.Polyline;

        resultGeometry.Paths.Count.ShouldBe(2);
        resultGeometry.Paths[0].Length.ShouldBe(5);
        resultGeometry.Paths[1].Length.ShouldBe(4);

        var ring = resultGeometry.Paths[1];

        ring[0].Equals(new EsriJson.Net.Geometry.RingPoint(10, 10)).ShouldBe(true);
        ring[1].Equals(new EsriJson.Net.Geometry.RingPoint(11, 10)).ShouldBe(true);
        ring[2].Equals(new EsriJson.Net.Geometry.RingPoint(11, 11)).ShouldBe(true);
        ring[3].Equals(new EsriJson.Net.Geometry.RingPoint(10, 11)).ShouldBe(true);
    }

    [Fact]
    public async Task Should_convert_polygon_to_esri_geometry() {
        var geometry = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0),
            ])
        );

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.Polygon>();
        var polygon = result as EsriJson.Net.Geometry.Polygon;

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
    public async Task Should_convert_polygon_with_holes_to_esri_geometry() {
        var geometry = new Polygon(
            new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0),
            ]), [
                new LinearRing([
                    new Coordinate(.5,.5),
                    new Coordinate(.6, .6),
                    new Coordinate(.4,.6),
                    new Coordinate(.5, .5)
                ])
            ]
        );

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.Polygon>();
        var polygon = result as EsriJson.Net.Geometry.Polygon;

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
    public async Task Should_convert_multipolygon_to_esri_geometry() {
        var geometry = new MultiPolygon(
            [new Polygon(new LinearRing([
                new Coordinate(0, 0),
                new Coordinate(1, 0),
                new Coordinate(1, 1),
                new Coordinate(0, 1),
                new Coordinate(0, 0),
            ])), new Polygon(new LinearRing([
                new Coordinate(.5,.5),
                new Coordinate(.6, .6),
                new Coordinate(.4,.6),
                new Coordinate(.5, .5)
            ]))
            ]
        );

        var computation = new NtsToEsriMapper.Computation(geometry);

        var handler = new NtsToEsriMapper.Handler(Logger);
        var result = await handler.Handle(computation, CancellationToken.None);

        result.ShouldBeOfType<EsriJson.Net.Geometry.Polygon>();
        var polygon = result as EsriJson.Net.Geometry.Polygon;

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
