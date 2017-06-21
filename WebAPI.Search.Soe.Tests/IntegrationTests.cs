using System.Net.Http;
using NUnit.Framework;
using Newtonsoft.Json;
using Soe.Common.Extensions;
using WebAPI.Domain.ArcServerInput;
using WebAPI.Domain.InputOptions;
using WebAPI.Search.Soe.Tests.Extensions;

namespace WebAPI.Search.Soe.Tests
{
    public class IntegrationTests
    {
        [TestFixture]
        public class BufferSearch
        {
            [Test]
            public void PointSearchWithBufferFindsOnePoint()
            {
                const string featureClass = "SGID10.ELEVATION.HighestPeaks";
                var args = new QueryArgs(featureClass, "point:[445190.8,4471328.55]", new[] {"NAME"}, "", 5);

                var client = new HttpClient();
                var url = "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/Search?{0}&f=json"
                    .With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith(
                    "{\"Results\":[{\"Geometry\":null,\"Attributes\":{\"Name\":\"Mount Timpanogos\"}}]").IgnoreCase);
            }

            [Test]
            public void PointSearchWithNoBufferFindsNothing()
            {
                const string featureClass = "SGID10.ELEVATION.HighestPeaks";
                var args = new QueryArgs(featureClass, "point:[445190.8,4471328.55]", new[] {"NAME"}, "");

                var client = new HttpClient();
                var url = "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/Search?{0}&f=json"
                    .With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("{\"Results\":[]").IgnoreCase);
            }
        }

        [TestFixture]
        public class PointInPoly
        {
            [Test]
            public void ReturnsErrorWhenFeatureClassDoesNotExist()
            {
                const string featureClass = "SGID93.Schema.Fake";
                var args = new QueryArgs(featureClass, "", new[] {"name"}, "");

                var client = new HttpClient();
                var url =
                    "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/Search?{0}&f=json".
                        With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result,
                            Is.EqualTo("{\"error\":{\"code\":400,\"message\":\"DBMS table not found\"}}").IgnoreCase);
            }

            [Test]
            public void SpatialQueryDatabaseForAttribute()
            {
                const string featureClass = "SGID93.BOUNDARIES.Counties";
                var args = new QueryArgs(featureClass, "point:[314755, 4609130]", new[] {"NAME"}, "");

                var client = new HttpClient();
                var url =
                    "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/Search?{0}&f=json".
                        With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result,
                             Does.StartWith(
                                "{\"Results\":[{\"Geometry\":null,\"Attributes\":{\"Name\":\"Box Elder\"}}]").IgnoreCase);
            }

            [Test]
            public void SpatialQueryDatabaseForShape()
            {
                const string featureClass = "SGID93.BOUNDARIES.Counties";
                var args = new QueryArgs(featureClass, "point:[314755, 4609130]", new[] {"shape@"}, "");

                var client = new HttpClient();
                var url =
                    "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/Search?{0}&f=json".
                        With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("{\"Results\":[{\"Geometry\":\"{\\\"rings\\\":[[[").IgnoreCase);
            }

            [Test]
            public void SpatialQueryDatabaseWithWhereClause()
            {
                const string featureClass = "SGID93.Boundaries.Counties";
                var args = new QueryArgs(featureClass, "point:[314755, 4609130]", new[] {"NAME, FIPS"},
                                         "name = 'not found'");

                var client = new HttpClient();
                var url =
                    "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/Search?{0}&f=json".
                        With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("{\"Results\":[]").IgnoreCase);
            }
        }

        public class QueryArgs
        {
            public QueryArgs(string featureClass, string geometry, string[] returnValues, string predicate,
                             double buffer = 0)
            {
                FeatureClass = featureClass;
                Geometry = geometry;
                ReturnValues = returnValues;
                WhereClause = predicate;
                Buffer = buffer;
            }

            /// <summary>
            ///     Gets or sets the feature class.
            /// </summary>
            /// <value>
            ///     The feature class name on which to search.
            /// </value>
            [JsonProperty("featureClass")]
            public string FeatureClass { get; set; }

            /// <summary>
            ///     Gets or sets the geometry.
            /// </summary>
            /// <value>
            ///     The geometry where the spatial query will search.
            /// </value>
            [JsonProperty("geometry")]
            public string Geometry { get; set; }

            /// <summary>
            ///     Gets or sets the return values.
            /// </summary>
            /// <value>
            ///     The return values are the attributes from the feature class to return.
            /// </value>
            [JsonProperty("returnValues")]
            public string[] ReturnValues { get; set; }

            /// <summary>
            ///     Gets or sets the where clause.
            /// </summary>
            /// <value>
            ///     The where clause to select features in the feature calss.
            /// </value>
            [JsonProperty("predicate")]
            public string WhereClause { get; set; }

            /// <summary>
            ///     Gets or sets the buffer.
            /// </summary>
            /// <value>
            ///     The buffer in meters to expand the point.
            /// </value>
            [JsonProperty("buffer")]
            public double Buffer { get; set; }
        }

        [TestFixture]
        public class ReverseMilepost
        {
            [Test]
            public void FindsMilepostOnFederalInterstate()
            {
                var args = new ReverseMilepostArgs(311770, 4510850, new ReverseMilepostOptions());

                var client = new HttpClient();
                var url = "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/ReverseMilepost?{0}&f=json"
                    .With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result,
                             Does.StartWith("{\"TopResult\":{\"Milepost\":42.848619712799547,\"Route\":\"0080N\"").
                               IgnoreCase);
            }

            [Test]
            public void ReprojectsPoints()
            {
                var args = new ReverseMilepostArgs(-111.90190785190332, 40.72597381846899,
                                                   new ReverseMilepostOptions {WkId = 4326});

                var client = new HttpClient();
                var url = "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/ReverseMilepost?{0}&f=json"
                    .With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith(
                    "{\"TopResult\":{\"Milepost\":17.42619992647089,\"Route\":\"0201P\"").IgnoreCase);
            }

            [Test]
            public void SkipsRampsAndFindsHighway()
            {
                var args = new ReverseMilepostArgs(423834.42, 4508729.44, new ReverseMilepostOptions());

                var client = new HttpClient();
                var url = "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/ReverseMilepost?{0}&f=json"
                    .With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith(
                    "{\"TopResult\":{\"Milepost\":17.426200270235142,\"Route\":\"0201P\"").IgnoreCase);
            }

            [Test]
            public void FailsGracefullyWhenNothingFound()
            {
                var args = new ReverseMilepostArgs(15, 15, new ReverseMilepostOptions());

                var client = new HttpClient();
                var url = "http://localhost:6080/arcgis/rest/services/Soe/SearchApi/MapServer/exts/Search/ReverseMilepost?{0}&f=json"
                    .With(args.ToQueryString());

                var response = client.GetAsync(url);
                var result = response.Result.Content.ReadAsStringAsync().Result;

                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.EqualTo("{\"TopResult\":null,\"EqualCandidates\":[]}"));

            }
        }
    }
}