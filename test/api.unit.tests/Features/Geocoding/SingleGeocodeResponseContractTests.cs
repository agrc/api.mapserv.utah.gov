using AGRC.api.Features.Geocoding;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using AGRC.api.Models.Constants;
using EsriJson.Net;
using NetTopologySuite.Features;

namespace api.tests.Features.Geocoding;
public class SingleGeocodeResponseContractTests {
    [Fact]
    public void Should_route_score_difference() {
        var model = new SingleGeocodeResponseContract {
            ScoreDifference = 1.1234
        };

        model.ScoreDifference.ShouldBe(1.12);
        model.ScoreDifference = 1.7;
        model.ScoreDifference.ShouldBe(1.7);
    }
    [Fact]
    public void Should_return_default_object_for_format_none() {
        var model = new SingleGeocodeResponseContract {
            Location = new Point(1, 1),
            Score = 100,
            MatchAddress = "Test Address",
            InputAddress = "Test Address"
        }.Convert(new() {
            Format = JsonFormat.None
        }, new(1));

        model.ShouldBeOfType<SingleGeocodeResponseContract>();
        model.ShouldBe(model);
    }
    [Fact]
    public void Should_return_default_object_for_format_default() {
        var model = new SingleGeocodeResponseContract {
            Location = new Point(1, 2),
            Score = 100,
            MatchAddress = "Test Address",
            InputAddress = "Test Address"
        }.Convert(new(), new(1));

        model.ShouldBeOfType<SingleGeocodeResponseContract>();
        model.ShouldBe(model);
    }
    [Fact]
    public void Should_return_graphic_for_esri_json() {
        const int score = 100;
        const LocatorType locatorType = LocatorType.RoadCenterlines;
        const int wkid = 4;

        var model = new SingleGeocodeResponseContract {
            Location = new Point(1, 2),
            Score = score,
            MatchAddress = "Test Address",
            InputAddress = "Test Address",
            AddressGrid = "Grid",
            Locator = locatorType.ToString(),
            ScoreDifference = 2
        }.Convert(new() {
            Format = JsonFormat.EsriJson,
            SpatialReference = wkid
        }, new(1));

        model.ShouldBeOfType<SerializableGraphic>();

        if (model is SerializableGraphic feature) {
            feature.Geometry.ShouldBeOfType<EsriJson.Net.Geometry.Point>();

            if (feature.Geometry is EsriJson.Net.Geometry.Point point) {
                point.X.ShouldBe(1);
                point.Y.ShouldBe(2);
            }

            feature.Attributes.Keys.ShouldBe(new[] { "Location", "Score", "Locator", "MatchAddress", "InputAddress", "StandardizedAddress", "AddressGrid", "ScoreDifference", "Wkid", "Candidates" });
            feature.Attributes["Location"].ShouldNotBeNull();
            feature.Attributes["Score"].ShouldBe(score);
            feature.Attributes["Locator"].ShouldBe(locatorType.ToString());
            feature.Attributes["MatchAddress"].ShouldBe("Test Address");
            feature.Attributes["InputAddress"].ShouldBe("Test Address");
            feature.Attributes["AddressGrid"].ShouldBe("Grid");
            feature.Attributes["ScoreDifference"].ShouldBe(2);
            feature.Attributes["Wkid"].ShouldBe(wkid);
            feature.Attributes["Candidates"].ShouldBeNull();
        }
    }
    [Fact]
    public void Should_return_graphic_for_geo_json() {
        const int score = 100;
        const LocatorType locatorType = LocatorType.RoadCenterlines;
        const int wkid = 4;

        var model = new SingleGeocodeResponseContract {
            Location = new Point(1, 2),
            Score = score,
            MatchAddress = "Test Address",
            InputAddress = "Test Address",
            AddressGrid = "Grid",
            Locator = locatorType.ToString()
        }.Convert(new() {
            Format = JsonFormat.GeoJson,
            SpatialReference = wkid
        }, new(1));

        model.ShouldBeOfType<Feature>();

        if (model is Feature feature) {
            feature.Geometry.ShouldBeOfType<NetTopologySuite.Geometries.Point>();

            if (feature.Geometry is NetTopologySuite.Geometries.Point point) {
                point.X.ShouldBe(1);
                point.Y.ShouldBe(2);
            }

            feature.Attributes.GetNames().ShouldBe(new[] { "Location", "Score", "Locator", "MatchAddress", "InputAddress", "AddressGrid", "ScoreDifference", "Wkid", "Candidates" });
            feature.Attributes["Location"].ShouldNotBeNull();
            feature.Attributes["Score"].ShouldBe(score);
            feature.Attributes["Locator"].ShouldBe(locatorType.ToString());
            feature.Attributes["MatchAddress"].ShouldBe("Test Address");
            feature.Attributes["InputAddress"].ShouldBe("Test Address");
            feature.Attributes["AddressGrid"].ShouldBe("Grid");
            feature.Attributes["ScoreDifference"].ShouldBe(-1);
            feature.Attributes["Wkid"].ShouldBe(wkid);
            feature.Attributes["Candidates"].ShouldBe(Array.Empty<Candidate>());
        }
    }
    [Fact]
    public void Should_return_graphic_for_esri_json_v2() {
        const int score = 100;
        const LocatorType locatorType = LocatorType.RoadCenterlines;
        const int wkid = 4;

        var model = new SingleGeocodeResponseContract {
            Location = new Point(1, 2),
            Score = score,
            MatchAddress = "Test Address",
            InputAddress = "Test Address",
            AddressGrid = "Grid",
            Locator = locatorType.ToString(),
            ScoreDifference = 2
        }.Convert(new() {
            Format = JsonFormat.EsriJson,
            SpatialReference = wkid
        }, new(2));

        model.ShouldBeOfType<SerializableGraphic>();

        if (model is SerializableGraphic feature) {
            feature.Geometry.ShouldBeOfType<EsriJson.Net.Geometry.Point>();

            if (feature.Geometry is EsriJson.Net.Geometry.Point point) {
                point.X.ShouldBe(1);
                point.Y.ShouldBe(2);
            }

            feature.Attributes.Keys.ShouldBe(new[] { "Score", "Locator", "MatchAddress", "InputAddress", "AddressGrid", "ScoreDifference" });
            feature.Attributes["Score"].ShouldBe(score);
            feature.Attributes["Locator"].ShouldBe(locatorType.ToString());
            feature.Attributes["MatchAddress"].ShouldBe("Test Address");
            feature.Attributes["InputAddress"].ShouldBe("Test Address");
            feature.Attributes["AddressGrid"].ShouldBe("Grid");
        }
    }
    [Fact]
    public void Should_return_graphic_for_geo_json_v2() {
        const int score = 100;
        const LocatorType locatorType = LocatorType.RoadCenterlines;
        const int wkid = 4;

        var model = new SingleGeocodeResponseContract {
            Location = new Point(1, 2),
            Score = score,
            MatchAddress = "Test Address",
            InputAddress = "Test Address",
            AddressGrid = "Grid",
            Locator = locatorType.ToString()
        }.Convert(new() {
            Format = JsonFormat.GeoJson,
            SpatialReference = wkid
        }, new(2));

        model.ShouldBeOfType<Feature>();

        if (model is Feature feature) {
            feature.Geometry.ShouldBeOfType<NetTopologySuite.Geometries.Point>();

            if (feature.Geometry is NetTopologySuite.Geometries.Point point) {
                point.X.ShouldBe(1);
                point.Y.ShouldBe(2);
            }

            feature.Attributes.GetNames().ShouldBe(new[] { "srid", "score", "locator", "matchAddress", "inputAddress", "addressGrid" });
            feature.Attributes["score"].ShouldBe(score);
            feature.Attributes["locator"].ShouldBe(locatorType.ToString());
            feature.Attributes["matchAddress"].ShouldBe("Test Address");
            feature.Attributes["inputAddress"].ShouldBe("Test Address");
            feature.Attributes["addressGrid"].ShouldBe("Grid");
            feature.Attributes["srid"].ShouldBe(wkid);
        }
    }
}
