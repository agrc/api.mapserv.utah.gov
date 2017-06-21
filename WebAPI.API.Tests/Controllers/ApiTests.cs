using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using NUnit.Framework;
using WebAPI.API.Controllers.API.Version1;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Tests.Controllers
{
    [TestFixture]
    public class ApiTests
    {
        [OneTimeSetUp]
        public void SetupGlobal()
        {
            CacheConfig.BuildCache();
        }


        [TestFixture]
        public class Search
        {
            [SetUp]
            public void Setup()
            {
                var config = new HttpConfiguration();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/");

                _controller = new SearchController
                    {
                        Request = request
                    };

                _controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            }

            private SearchController _controller;

            private static ResultContainer<List<SearchResult>> GetResultContent(Task<HttpResponseMessage> response)
            {
                //Debug.Print(response.Result.Content.ReadAsStringAsync().Result);

                return response.Result.Content.ReadAsAsync<ResultContainer<List<SearchResult>>>(new[]
                    {
                        new TextPlainResponseFormatter()
                    }).Result;
            }

            [Test]
            public void CanQueryWithBuffer()
            {
                var response = _controller.Get(featureClass: "SGID10.Elevation.HighestPeaks",
                                               options: new SearchOptions
                                                   {
                                                       Buffer = 5,
                                                       Geometry = "point:[445190.8,4471328.55]"
                                                   }, returnValues: "name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Count, Is.EqualTo(1));
            }

            [Test]
            public void CanQueryForAllAttributesWithEmptyPredicate()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions(), returnValues: "name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Count, Is.EqualTo(29));
            }

            [Test]
            public void CanQueryForMultipleAttributesPredicate()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions(), returnValues: "name,fips");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Count, Is.EqualTo(29));
            }

            [Test]
            public void CanQueryForSpecificAttributes()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions
                                                   {
                                                       Predicate = "name like 'k%'",
                                                   },
                                               returnValues: "name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Count, Is.EqualTo(1));
            }

            [Test]
            public void CanQueryPointInPolygon()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions
                                                   {
                                                       Geometry = "point:[447158, 4493466]"
                                                   },
                                               returnValues: "name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
            }

            [Test]
            public void CanReturnShape()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions
                                               {
                                                   Geometry = "point:[447158,4493466]"
                                               },
                                               returnValues: "shape@");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.First().Geometry, Is.Not.Null);
            }

            [Test]
            public void CanQueryWithDifferentWkId()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions
                                               {
                                                   Geometry = "point:[-111.972634140647, 41.221062546402]",
                                                   WkId = 4326
                                               },
                                               returnValues: "name");
                
                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.First().Attributes.First().Value, Is.EqualTo("WEBER"));
            }

            [Test]
            public void CanReturnShapeWithDifferentWkId()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions
                                               {
                                                   Geometry = "point:[-111.972634140647, 41.221062546402]",
                                                   WkId = 4326
                                               },
                                               returnValues: "shape@");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.First().Geometry["rings"][0][0][0].ToString(), Does.Contain("-111."));
            }

            [Test]
            public void DoNothingIfFeatureClassIsNull()
            {
                var response = _controller.Get(featureClass: "",
                                               options: new SearchOptions(), returnValues: "name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message, Does.StartWith("featureClass is a required field. Input was empty."));
            }

            [Test]
            public void DoNothingIfReturnValuesIsNull()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions(), returnValues: "");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message, Does.StartWith("returnValues is a required field. Input was empty."));
            }

            [Test]
            public void BlockSqlInjectionAttempts()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions
                                                   {
                                                       Predicate = "; drop table SGID10.Boundaries.Counties--"
                                                   }, returnValues: "name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message, Does.StartWith("Predicate contains unsafe characters. Don't be a jerk."));

            }

            [Test]
            public void ReturnsErrorWhenAttributeNotFoundForStraightSql()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions(), returnValues: "nam, barf, name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message,
                            Is.EqualTo(
                                "SGID10.Boundaries.Counties does not contain an attribute 'nam' or 'barf'. Check your spelling.").IgnoreCase);
            }

            [Test]
            public void ReturnsErrorWhenAttributeNotFoundFromSoeWithShape()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions(), returnValues: "nam,barf,name,shape@");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message,
                            Is.EqualTo(
                                "SGID10.Boundaries.Counties does not contain an attribute 'nam' or 'barf'. Check your spelling.").IgnoreCase);
            }

            [Test]
            public void ReturnsErrorWhenAttributeNotFoundFromSoe()
            {
                var response = _controller.Get(featureClass: "SGID10.Boundaries.Counties",
                                               options: new SearchOptions(), returnValues: "nam,barf,name,shape@");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message,
                            Is.EqualTo(
                                "SGID10.Boundaries.Counties does not contain an attribute 'nam' or 'barf'. Check your spelling.").IgnoreCase);
            }

            [Test]
            public void ReturnsErrorWhenFeatureClassDoesNotExistForStraightSql()
            {
                var response = _controller.Get(featureClass: "SGID10.Schema.Fake",
                                              options: new SearchOptions(), returnValues: "name");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message,
                            Is.EqualTo(
                                "SGID10.Schema.Fake probably does not exist. Check your spelling.").IgnoreCase);
            }

            [Test]
            public void ReturnsErrorWhenFeatureClassDoesNotExistForSpatialSearch()
            {
                var response = _controller.Get(featureClass: "SGID10.Schema.Fake",
                                              options: new SearchOptions(), returnValues: "shape@");

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.BadRequest));
                Assert.That(result.Result, Is.Null);
                Assert.That(result.Message,
                            Is.EqualTo(
                                "SGID10.Schema.Fake does not exist. Check your spelling.").IgnoreCase);
            }
        }

        [TestFixture]
        public class Geocoding
        {
            [SetUp]
            public void Setup()
            {
                var config = new HttpConfiguration();
                var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/");

                _controller = new GeocodeController
                {
                    Request = request
                };

                _controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            }

            private GeocodeController _controller;

            private static ResultContainer<GeocodeAddressResult> GetResultContent(HttpResponseMessage response)
            {
                //Debug.Print(response.Result.Content.ReadAsStringAsync().Result);

                return response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>(new[]
                    {
                        new TextPlainResponseFormatter()
                    }).Result;
            }

            [Test]
            public void CanGeocodeStreetDirectionConcatenatedAddresses()
            {
                var response = _controller.Get(street: "1991S 700E", zone: "SLC", options: new GeocodeOptions());

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Score, Is.EqualTo(100));
            }

            [Test]
            public void HighwayGeocoding()
            {
               var response = _controller.Get(street: "7677 S US 89", zone: "SLC", options: new GeocodeOptions());

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Score, Is.EqualTo(100));
            }

            [Test]
            public void Issue22WithHashInUrl()
            {
                var response = _controller.Get(street: "4973 SO MURRAY BLVD #N24", zone: "84123", options: new GeocodeOptions
                {
                    WkId = 4326
                });

                var result = GetResultContent(response);

                Assert.That(result.Status, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(result.Result, Is.Not.Null);
                Assert.That(result.Result.Location.X > -112 && result.Result.Location.X < -111, Is.True);
                Assert.That(result.Result.Location.Y > 39 && result.Result.Location.Y < 41, Is.True);
                Assert.That(result.Result.Score, Is.GreaterThan(80)); 
            }
        }
    }
}