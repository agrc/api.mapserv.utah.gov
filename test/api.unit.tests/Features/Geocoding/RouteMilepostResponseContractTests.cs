using AGRC.api.Features.Milepost;
using AGRC.api.Models;
using AGRC.api.Models.Constants;
using EsriJson.Net;
using NetTopologySuite.Features;

namespace api.tests.Features.Milepost;

public class RouteMilepostResponseContractTests {
    [Fact]
    public void Should_return_same_object_when_format_is_none() {
        var contract = new RouteMilepostResponseContract("source", new Point(1, 2), "matchRoute");

        var result = contract.Convert(new RouteMilepostRequestOptionsContract {
            Format = JsonFormat.None,
        }, new ApiVersion(1, 0));

        result.ShouldBeOfType<RouteMilepostResponseContract>();
        result.ShouldBeEquivalentTo(contract);
    }
    [Fact]
    public void Should_convert_to_esri_json() {
        var contract = new RouteMilepostResponseContract("source", new Point(1, 2), "matchRoute");

        var result = contract.Convert(new RouteMilepostRequestOptionsContract {
            Format = JsonFormat.EsriJson,
        }, new ApiVersion(1, 0));

        result.ShouldBeOfType<SerializableGraphic>();
        var graphic = (SerializableGraphic)result;
        graphic.Attributes.Count.ShouldBe(4);
        graphic.Attributes.Keys.ShouldContain("Source");
        graphic.Attributes.Keys.ShouldContain("Location");
        graphic.Attributes.Keys.ShouldContain("MatchRoute");
        graphic.Attributes.Keys.ShouldContain("InputRouteMilePost");

        graphic.Geometry.ShouldBeOfType<EsriJson.Net.Geometry.Point>();
        var geometry = (EsriJson.Net.Geometry.Point)graphic.Geometry;
        geometry.X.ShouldBe(1);
        geometry.Y.ShouldBe(2);
        geometry.CRS.WellKnownId.ShouldBe(26912);
    }
    [Fact]
    public void Should_create_empty_graphic_if_geometry_is_null() {
        var contract = new RouteMilepostResponseContract(null, null, null);

        var result = contract.Convert(new RouteMilepostRequestOptionsContract {
            Format = JsonFormat.EsriJson,
        }, new ApiVersion(1, 0));

        result.ShouldBeOfType<SerializableGraphic>();
        var graphic = (SerializableGraphic)result;
        graphic.Geometry.ShouldBeNull();
    }
    [Fact]
    public void Should_convert_to_esri_json_with_less_attributes_in_version_2() {
        var contract = new RouteMilepostResponseContract(null, new Point(1, 2), "matchRoute");

        var result = contract.Convert(new RouteMilepostRequestOptionsContract {
            Format = JsonFormat.EsriJson,
        }, new ApiVersion(2, 0));

        result.ShouldBeOfType<SerializableGraphic>();
        var graphic = (SerializableGraphic)result;
        graphic.Attributes.Count.ShouldBe(1);
        graphic.Attributes.Keys.ShouldContain("MatchRoute");

        graphic.Geometry.ShouldBeOfType<EsriJson.Net.Geometry.Point>();
        var geometry = (EsriJson.Net.Geometry.Point)graphic.Geometry;
        geometry.X.ShouldBe(1);
        geometry.Y.ShouldBe(2);
        geometry.CRS.WellKnownId.ShouldBe(26912);
    }
    [Fact]
    public void Should_convert_to_geo_json() {
        var contract = new RouteMilepostResponseContract("source", new Point(1, 2), "matchRoute");

        var result = contract.Convert(new RouteMilepostRequestOptionsContract {
            Format = JsonFormat.GeoJson,
        }, new ApiVersion(1, 0));

        result.ShouldBeOfType<Feature>();
        var feature = (Feature)result;
        feature.Attributes.Count.ShouldBe(4);
        feature.Attributes.GetNames().ShouldContain("Source");
        feature.Attributes.GetNames().ShouldContain("Location");
        feature.Attributes.GetNames().ShouldContain("MatchRoute");
        feature.Attributes.GetNames().ShouldContain("InputRouteMilePost");

        feature.Geometry.ShouldBeOfType<NetTopologySuite.Geometries.Point>();
        var geometry = (NetTopologySuite.Geometries.Point)feature.Geometry;
        geometry.X.ShouldBe(1);
        geometry.Y.ShouldBe(2);
        geometry.SRID.ShouldBe(26912);
    }
    [Fact]
    public void Should_create_empty_feature_if_geometry_is_null() {
        var contract = new RouteMilepostResponseContract(null, null, null);

        var result = contract.Convert(new RouteMilepostRequestOptionsContract {
            Format = JsonFormat.GeoJson,
        }, new ApiVersion(1, 0));

        result.ShouldBeOfType<Feature>();
        var feature = (Feature)result;
        feature.Geometry.ShouldBeNull();
    }
    [Fact]
    public void Should_convert_to_geo_json_with_less_attributes_in_version_2() {
        var contract = new RouteMilepostResponseContract(null, new Point(1, 2), "matchRoute");

        var result = contract.Convert(new RouteMilepostRequestOptionsContract {
            Format = JsonFormat.GeoJson,
        }, new ApiVersion(2, 0));

        result.ShouldBeOfType<Feature>();
        var feature = (Feature)result;
        feature.Attributes.Count.ShouldBe(2);
        feature.Attributes.GetNames().ShouldContain("matchRoute");
        feature.Attributes.GetNames().ShouldContain("srid");

        feature.Geometry.ShouldBeOfType<NetTopologySuite.Geometries.Point>();
        var geometry = (NetTopologySuite.Geometries.Point)feature.Geometry;
        geometry.X.ShouldBe(1);
        geometry.Y.ShouldBe(2);
        geometry.SRID.ShouldBe(26912);
    }
}
