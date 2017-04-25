using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using NUnit.Framework;
using WebAPI.API.Commands.Geocode;
using WebAPI.API.Controllers.API.Version1;
using WebAPI.Common.Executors;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Tests.Controllers
{
    [TestFixture]
    public class StreetZoneGeocodeEndpointTests : GeocodeTestsBase
    {
        GeocodeController _controller;

        [SetUp]
        public void Setup()
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/user/1337");

            _controller = new GeocodeController
            {
                Request = request
            };

            _controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }

        [Test]
        public void MultipleRequestsHappenFast()
        {
            var addresses = new List<TestAddress>
            {
                new TestAddress
                {
                    Id = 1,
                    Street = "160 w main st",
                    Zone = "84652",
                    KnownScore = 100,
                    KnownLocation = new Location
                    {
                        X = 424888.4,
                        Y = 4317812.22
                    }
                },
                new TestAddress
                {
                    Id = 2,
                    Street = "80 w 100 s",
                    Zone = "84754",
                    KnownScore = 100,
                    KnownLocation = new Location
                    {
                        X = 404232.41,
                        Y = 4284199.87
                    }
                },
                new TestAddress
                {
                    Id = 3,
                    Zone = "84754",
                    KnownLocation = new Location
                    {
                        X = 404244.06,
                        Y = 4284146.85
                    },
                    KnownScore = 100,
                    Street = "83 W 100 S"
                },
                new TestAddress
                {
                    Id = 4,
                    Zone = "84754",
                    KnownLocation = new Location
                    {
                        X = 404410.09,
                        Y = 4284257.9
                    },
                    KnownScore = 100,
                    Street = "64 S Main St"
                },
                new TestAddress
                {
                    Id = 5,
                    Zone = "84620",
                    KnownLocation = new Location
                    {
                        X = 418711.83,
                        Y = 4307825.21
                    },
                    KnownScore = 100,
                    Street = "360 S Main St"
                }
            };

            var results2 = new List<GeocodeAddressResult>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            addresses.ForEach(
                _ =>
                results2.Add(
                    CommandExecutor.ExecuteCommand(new OrchesrateeGeocodeCommand(_.Street, _.Zone, new GeocodeOptions())
                    {
                        Testing = true
                    })));

            stopwatch.Stop();
            Debug.Print("======================New Way: "
                        + stopwatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));

            //for (var i = 0; i < results.Count; i++)
            //{
            //    Assert.That(results2[i].Score, Is.GreaterThanOrEqualTo(addresses[i].KnownScore));
            //    Assert.That(results2[i].Location.X, Is.EqualTo(addresses[i].KnownLocation.X));
            //    Assert.That(results2[i].Location.Y, Is.EqualTo(addresses[i].KnownLocation.Y));
            //}
        }

        [Test]
        public void RequestFailsWithNoApiKeyInQueryString()
        {
            //arrange
            var client = new HttpClient();

            //act
            var response = client.GetAsync("http://webapi/api/v1/Geocode/326 east south temple/84111").Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>().Result.Status,
                        Is.EqualTo((int)HttpStatusCode.BadRequest), "no api key");
        }

        [Test]
        public void RequestGeocodesAKnownAddressCorrectly()
        {
            //arrange
            var client = new HttpClient();

            //act
            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/326 east south temple/84111?apiKey=AGRC-Explorer").Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Status, Is.EqualTo((int) HttpStatusCode.OK), "has api key");
            Assert.That(resultContainer.Result.Score, Is.GreaterThan(80), "address is found");
            Assert.That(resultContainer.Result.MatchAddress, Is.EqualTo("326 E SOUTH TEMPLE ST, salt lake city").IgnoreCase, "address is found");
        }

        [Test]
        public void RequestReturnsCandidates()
        {
            //arrange
            var client = new HttpClient();

            //act
            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/326 east south temple/84111?apiKey=AGRC-Explorer&suggest=1")
                      .Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Status, Is.EqualTo((int) HttpStatusCode.OK), "has api key");
            Assert.That(resultContainer.Result.Score, Is.GreaterThan(80), "address is found");
            Assert.That(resultContainer.Result.MatchAddress.ToLowerInvariant(), Is.EqualTo("326 E SOUTH TEMPLE ST, salt lake city".ToLowerInvariant()), "address is found");
            Assert.That(resultContainer.Result.Candidates, Is.Not.Empty, "Should be 1 if there are 1.");
            Assert.That(resultContainer.Result.Candidates.Count(), Is.EqualTo(1), "Should be 1 if there are 1.");
        }

        [Test]
        public void RequestReturnsCandidatesWithWeightDeterminingMatch()
        {
            //arrange
            var client = new HttpClient();

            //act
            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/100 south main/84664?apiKey=AGRC-Explorer&suggest=3")
                      .Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Status, Is.EqualTo((int)HttpStatusCode.OK), "has api key");
            Assert.That(resultContainer.Result.Score, Is.GreaterThan(80), "address is found");
            Assert.That(resultContainer.Result.MatchAddress.ToLowerInvariant(), Is.EqualTo("100 S MAIN ST, mapleton".ToLowerInvariant()), "address is found");
            Assert.That(resultContainer.Result.Candidates, Is.Not.Empty, "Should be what you suggest.");
            Assert.That(resultContainer.Result.Candidates.Count(), Is.EqualTo(3), "Should be what you suggest.");
        }

        [Test]
        public void RequestWithCallbackReturnsJsonP()
        {
            //arrange
            var client = new HttpClient();

            //act
            var response =
                client.GetAsync(
                    "http://webapi/api/v1/Geocode/326 east south temple/84111?apiKey=AGRC-Explorer&callback=callback")
                      .Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsStringAsync().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.StartsWith("callback"), "Should padd with callback function.");
        }

        [Test]
        public void DeliveryPointWithZipInput()
        {
            var response = _controller.Get("who cares", "84201", new GeocodeOptions());

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            Assert.That(response.IsSuccessStatusCode);

            var actual = response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>().Result.Result;

            Assert.That(actual.MatchAddress, Is.EqualTo("INTERNAL REVENUE SERVICE 324 25TH ST"));
            Assert.That(actual.Wkid, Is.EqualTo(26912));
            Assert.That(Math.Round(actual.Location.X, 2), Is.EqualTo(418473.37));
            Assert.That(Math.Round(actual.Location.Y, 2), Is.EqualTo(4563753.59));
        }

        [Test]
        public void DeliveryPointWithZipInput4326()
        {
            var response = _controller.Get("dopesn't matter", "84201", new GeocodeOptions
                {
                    WkId = 4326
                });

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            Assert.That(response.IsSuccessStatusCode);

            var actual = response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>().Result.Result;

            Assert.That(actual.MatchAddress, Is.EqualTo("INTERNAL REVENUE SERVICE 324 25TH ST"));
            Assert.That(actual.Wkid, Is.EqualTo(4326));
            Assert.That(Math.Round(actual.Location.X, 2), Is.EqualTo(-111.97));
            Assert.That(Math.Round(actual.Location.Y, 2), Is.EqualTo(41.22));
        }

        [Test]
        public void DeliveryPointWithPlaceInput()
        {
            var response = _controller.Get("2001 S STATE ST", "SLC", new GeocodeOptions());

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            Assert.That(response.IsSuccessStatusCode);
        }
    }
}