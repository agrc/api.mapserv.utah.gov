using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Hosting;
using CsvHelper;
using NUnit.Framework;
using WebAPI.API.Controllers.API.Version1;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Tests
{
    [TestFixture]
    public class GeocodingBaselineTests
    {
        public class CsvTestAddress
        {
            public string Address { get; set; }
            public string Zone { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
        }

        public class TestAddressFactory
        {
            public static IEnumerable ZipCodes
            {
                get
                {
                    var csvfile = Path.Combine(TestContext.CurrentContext.TestDirectory, "baseline_zipcodes.csv");
                    using (var file = File.OpenText(csvfile))
                    {
                        var csv = new CsvReader(file);
                        var addresses = csv.GetRecords<CsvTestAddress>();

                        foreach (var address in addresses)
                        {
                            yield return new TestCaseData(address).SetName(CreateTestName(address.Address, address.Zone));
                        }
                    }
                }
            }

            public static IEnumerable CityNames
            {
                get
                {
                    var csvfile = Path.Combine(TestContext.CurrentContext.TestDirectory, "baseline_citynames.csv");
                    using (var file = File.OpenText(csvfile))
                    {
                        var csv = new CsvReader(file);
                        var addresses = csv.GetRecords<CsvTestAddress>();

                        foreach (var address in addresses)
                        {
                            yield return new TestCaseData(address).SetName(CreateTestName(address.Address, address.Zone));
                        }
                    }
                }
            }

            private static string CreateTestName(string address, string zone)
            {
                return string.Concat(address.Where(char.IsLetterOrDigit)) + "_" + string.Concat(zone.Where(char.IsLetterOrDigit));
            }
        }

        private static GeocodeController _controller;

        private static ResultContainer<GeocodeAddressResult> GetResultContent(HttpResponseMessage response)
        {
            //Debug.Print(response.Result.Content.ReadAsStringAsync().Result);

            return response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>(new[]
            {
                new TextPlainResponseFormatter()
            }).Result;
        }

        private static double GetDistanceBetween(Location point, double x2, double y2)
        {
            var dx = point.X - x2;
            var dy = point.Y - y2;

            var d = Math.Pow(dx, 2) + Math.Pow(dy, 2);
            var distance = Math.Sqrt(d);

            return distance;
        }

        public List<CsvTestAddress> ZipCodes { get; set; }
        [OneTimeSetUp]
        public void SetupGlobal()
        {
            CacheConfig.BuildCache();
        }

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

        [Test, TestCaseSource(typeof(TestAddressFactory), "ZipCodes")]
        [TestCaseSource(typeof(TestAddressFactory), "CityNames")]
        public void BaselineTests(CsvTestAddress address)
        {
            var response = _controller.Get(address.Address, address.Zone, new GeocodeOptions());
            var result = GetResultContent(response);

            Assert.That(result.Status, Is.EqualTo((int) HttpStatusCode.OK));
            Assert.That(result.Result, Is.Not.Null);

            var distanceBetween = GetDistanceBetween(result.Result.Location, address.X, address.Y);

            Assert.That(distanceBetween, Is.LessThan(100));
        }
    }
}
