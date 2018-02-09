using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using AutoFixture;
using WebAPI.Domain;
using WebAPI.Domain.Addresses;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.EndpointArgs;

namespace WebAPI.API.Tests.Controllers
{
    [TestFixture]
    public class MultiGeocodeEndpointTests
    {
        [Test, Explicit]
        public void GetSerializedJson()
        {
            //act
            var multiGeocodeContainer = new MultipleGeocodeContainerArgs
            {
                Addresses = new List<AddressWithId>
                {
                    new AddressWithId
                    {
                        Id = 1,
                        Street = "160 w main st",
                        Zone = "84652",
                    },
                    new AddressWithId
                    {
                        Id = 2,
                        Street = "80 w 100 s",
                        Zone = "84754",
                    },
                    new AddressWithId
                    {
                        Id = 3,
                        Zone = "84754",
                        Street = "83 W 100 S"
                    },
                    new AddressWithId
                    {
                        Id = 4,
                        Zone = "84754",
                        Street = "64 S Main St"
                    },
                    new AddressWithId
                    {
                        Id = 5,
                        Zone = "84620",
                        Street = "360 S Main St"
                    },
                    new AddressWithId
                    {
                        Id = 6,
                        Zone = "84104",
                        Street = "1 state office building"
                    }
                }
            };

            var serializer = JsonConvert.SerializeObject(multiGeocodeContainer,
                                                         new JsonSerializerSettings());

            Debug.Print(serializer);
        }

        [Test]
        public void MultipleAddressGetGeocodedAndIdsMatch()
        {
            //arrange
            var client = new HttpClient();

            //act
            var multiGeocodeContainer = new MultipleGeocodeContainerArgs
            {
                Addresses = new List<AddressWithId>
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
                            X = 418702.33,
                            Y = 4307790.93
                        },
                        KnownScore = 100,
                        Street = "360 S Main St"
                    },
                    new TestAddress
                    {
                        Id = 6,
                        Zone = "84104",
                        Street = "1 state office building",
                        KnownScore = 0
                    }
                }
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var response =
                client.PostAsJsonAsync("http://webapi/api/v1/Geocode/multiple?apiKey=AGRC-Explorer",
                                       multiGeocodeContainer).Result;
            stopwatch.Stop();

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            Debug.Print(stopwatch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture));

            //assert
            var resultContainer =
                response.Content.ReadAsAsync<ResultContainer<MultipleGeocdeAddressResultContainer>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Status, Is.EqualTo((int) HttpStatusCode.OK), "api key");
            Assert.That(resultContainer.Result.Addresses.Count, Is.EqualTo(multiGeocodeContainer.Addresses.Count),
                        "counts sbould be equal");

            var first = multiGeocodeContainer.Addresses.First(x => x.Id == 1);
            Assert.That(resultContainer.Result.Addresses.First(x => x.Id == 1).InputAddress,
                        Is.EqualTo(first.Street + ", " + first.Zone), "streets are the same?");
        }

        [Test]
        public void RequestFailsWithNoApiKeyInQueryString()
        {
            //arrange
            var client = new HttpClient();

            //act
            var response =
                client.PostAsJsonAsync("http://webapi/api/v1/Geocode/multiple",
                                       new MultipleGeocodeContainerArgs
                                       {
                                           Addresses = new List<AddressWithId>
                                           {
                                               new AddressWithId
                                               {
                                                   Id = 1,
                                                   Street = "street",
                                                   Zone = "zone"
                                               }
                                           }
                                       }).Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content.ReadAsAsync<ResultContainer<MultipleGeocodeContainerArgs>>().Result.Status,
                        Is.EqualTo((int)HttpStatusCode.BadRequest), "no api key");
        }

        [Test]
        public void RequestReturnsSameNumberOfResultsBelowMaxAllowed()
        {
            //arrange
            var fixture = new Fixture();
            var client = new HttpClient();

            //act
            var multiGeocodeContainer = new MultipleGeocodeContainerArgs
            {
                Addresses = fixture.CreateMany<AddressWithId>(8).ToList()
            };

            var response =
                client.PostAsJsonAsync("http://webapi/api/v1/Geocode/multiple?apiKey=AGRC-Explorer",
                                       multiGeocodeContainer).Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsAsync<ResultContainer<MultipleGeocodeContainerArgs>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Status, Is.EqualTo((int) HttpStatusCode.OK), "api key");
            Assert.That(resultContainer.Result.Addresses.Count, Is.EqualTo(multiGeocodeContainer.Addresses.Count),
                        "same number as passed in");
        }

        [Test]
        public void RequestSkipsIdsThatAreNotUnique()
        {
            //arrange
            var fixture = new Fixture();
            var client = new HttpClient();
            var multiGeocodeContainer = new MultipleGeocodeContainerArgs
            {
                Addresses = fixture.Build<AddressWithId>().With(x => x.Id, 1).CreateMany(2).ToList()
            };

            fixture.AddManyTo(multiGeocodeContainer.Addresses, 8);

            Assert.That(multiGeocodeContainer.Addresses.Count, Is.EqualTo(10), "test data");
            Assert.That(multiGeocodeContainer.Addresses.Count(x => x.Id == 1), Is.EqualTo(3),
                        "test data has right amount of duplicates");

            //act
            var response =
                client.PostAsJsonAsync("http://webapi/api/v1/Geocode/multiple?apiKey=AGRC-Explorer",
                                       multiGeocodeContainer).Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsAsync<ResultContainer<MultipleGeocodeContainerArgs>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Status, Is.EqualTo((int) HttpStatusCode.OK), "api key");
            Assert.That(resultContainer.Result.Addresses.Count, Is.EqualTo(8), "trim duplicate id's");
            Assert.That(resultContainer.Result.Addresses.Count(x => x.Id > 100), Is.EqualTo(0), "uses first set of id's");
        }

        [Test]
        public void RequestTruncatesReponseToMaxAllowedAddresses()
        {
            //arrange
            var fixture = new Fixture();
            var client = new HttpClient();

            //act
            var multiGeocodeContainer = new MultipleGeocodeContainerArgs
            {
                Addresses = fixture.Build<AddressWithId>().CreateMany(110).ToList()
            };

            var response =
                client.PostAsJsonAsync("http://webapi/api/v1/Geocode/multiple?apiKey=AGRC-Explorer",
                                       multiGeocodeContainer).Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsAsync<ResultContainer<MultipleGeocodeContainerArgs>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Status, Is.EqualTo((int) HttpStatusCode.OK), "api key");
            Assert.That(resultContainer.Result.Addresses.Count, Is.EqualTo(100), "maxed out");
            Assert.That(resultContainer.Result.Addresses.Count(x => x.Id > 100), Is.EqualTo(0), "uses first set of id's");
        }

        [Test]
        public void RequestWithCallbackReturnsError()
        {
            //arrange
            var fixture = new Fixture();
            var client = new HttpClient();

            //act
            var multiGeocodeContainer = new MultipleGeocodeContainerArgs
            {
                Addresses = fixture.CreateMany<AddressWithId>(8).ToList()
            };

            var response =
                client.PostAsJsonAsync(
                    "http://webapi/api/v1/Geocode/multiple?apiKey=AGRC-Explorer&callback=cb",
                    multiGeocodeContainer).Result;

            Debug.Print(response.Content.ReadAsStringAsync().Result);

            //assert
            var resultContainer = response.Content.ReadAsAsync<ResultContainer<MultipleGeocodeContainerArgs>>().Result;

            Assert.That(response.StatusCode == HttpStatusCode.OK);
            Assert.That(resultContainer.Message.Contains("does not work"));
            Assert.That(resultContainer.Status, Is.EqualTo(400));
        }
    }
}