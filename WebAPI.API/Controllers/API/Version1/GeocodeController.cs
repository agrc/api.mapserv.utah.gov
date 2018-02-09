using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Ninject.Infrastructure.Language;
using Serilog;
using WebAPI.API.Commands.Address;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Exceptions;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
using WebAPI.Common.Formatters;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerInput;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.DataStructures;
using WebAPI.Domain.EndpointArgs;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Controllers.API.Version1
{
    public class GeocodeController : ApiController
    {
        private static readonly string NotifyEmails = ConfigurationManager.AppSettings["notify_email"];
        private const int MaxAddresses = 100;

        [HttpGet]
        public HttpResponseMessage Get(string street, string zone, [FromUri] GeocodeOptions options)
        {
            #region validation

            var errors = "";
            if (string.IsNullOrEmpty(street))
            {
                errors = "Street is empty.";
            }

            if (string.IsNullOrEmpty(zone))
            {
                errors += "Zip code or city name is emtpy";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<GeocodeAddressResult>
                                                  {
                                                      Status = (int) HttpStatusCode.BadRequest,
                                                      Message = errors
                                                  });
            }

            street = street.Trim();
            zone = zone.Trim();
            #endregion

            GeocodeAddressResult geocodeAddressResult;

            try
            {
                geocodeAddressResult =
                    CommandExecutor.ExecuteCommand(new OrchesrateeGeocodeCommand(street, zone, options));
            }
            catch (AggregateException ex)
            {
                Log.Error("Geocoding error occurred.", ex);
                var errorList = new List<string>();

                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    if (e is GeocodingException)
                    {
                        errorList.Add(e.Message);
                        CommandExecutor.ExecuteCommand(new NotifyGeocoderDownCommand(e.Message, NotifyEmails));
                    }
                    else
                    {
                        errorList.Add(e.Message);
                    }

                    return Request.CreateResponse(HttpStatusCode.InternalServerError,
                            new ResultContainer<GeocodeAddressResult>
                            {
                                Status = (int)HttpStatusCode.InternalServerError,
                                Message = $"Geocoding error occured. {string.Join(". ", errorList)}",
                                Result = new GeocodeAddressResult
                                {
                                    InputAddress = $"{street}, {zone}",
                                    Score = -9
                                }
                            })
                        .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));
                }

                return null;
            }
            catch (Exception)
            {
                // test
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            if (geocodeAddressResult == null || geocodeAddressResult.Score < 0)
            {
                Log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
                         options.AcceptScore);

                return Request.CreateResponse(HttpStatusCode.NotFound,
                                                            new ResultContainer<GeocodeAddressResult>
                                                                {
                                                                    Status = (int) HttpStatusCode.NotFound,
                                                                    Message = $"No address candidates found with a score of {options.AcceptScore} or better."
                                                                })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));
            }

            if (geocodeAddressResult.Location == null)
            {
                Log.Warning("Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
                         options.AcceptScore);
            }

            geocodeAddressResult.Wkid = options.WkId;

            var response = Request.CreateResponse(HttpStatusCode.OK,
                                                                new ResultContainer<GeocodeAddressResult>
                                                                    {
                                                                        Status = (int) HttpStatusCode.OK,
                                                                        Result = geocodeAddressResult
                                                                    })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Multiple(MultipleGeocodeContainerArgs addresseses, [FromUri] GeocodeOptions options)
        {
            var notifications = "";

            #region validation

            if (addresseses == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                                            new ResultContainer<MultipleGeocdeAddressResultContainer>
                                                                {
                                                                    Status = (int) HttpStatusCode.BadRequest,
                                                                    Message =
                                                                        "Could not deserialize json. Please validate json to make sure it is in correct format. Try http://jsonlint.com"
                                                                })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<MultipleGeocdeAddressResultContainer>));
            }

            if (!addresseses.Addresses.Any())
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                                            new ResultContainer<MultipleGeocdeAddressResultContainer>
                                                                {
                                                                    Status = (int) HttpStatusCode.BadRequest,
                                                                    Message =
                                                                        "No addresses to geocode. Please validate json to make sure it is in correct format. Try http://jsonlint.com"
                                                                })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<MultipleGeocdeAddressResultContainer>));
            }

            if (addresseses.Addresses.Count > MaxAddresses)
            {
                notifications = string.Format("Input {0}/{1} - Only processed {1}. ", addresseses.Addresses.Count,
                                              MaxAddresses);
                addresseses.Addresses = addresseses.Addresses.OrderBy(x => x.Id).Take(MaxAddresses).ToList();
            }

            var duplicateIds = addresseses.Addresses.GroupBy(x => x.Id)
                                          .Where(x => x.Count() > 1)
                                          .Select(x => x.Key)
                                          .ToList();

            if (duplicateIds.Count > 0)
            {
                notifications +=
                    string.Format("Found duplicate id's in list for ({0}). These addresses will not be geocoded.",
                                  string.Join(", ", duplicateIds));
            }

            #endregion

            var batchAddressResults = new Dictionary<int, GeocodeAddressResult>();

            foreach (var address in addresseses.Addresses)
            {
                if (duplicateIds.Contains(address.Id))
                {
                    continue;
                }

                batchAddressResults.Add(address.Id,
                                        CommandExecutor.ExecuteCommand(new OrchesrateeGeocodeCommand(address.Street,
                                                                                                     address.Zone,
                                                                                                     options)));
            }

            var result = new MultipleGeocdeAddressResultContainer
                {
                    Addresses = batchAddressResults.Select(MultipleGeocodeAddressResult.MapResult).ToList()
                };

            duplicateIds.ForEach(x => result.Addresses.Add(new MultipleGeocodeAddressResult
                {
                    Id = x,
                    Score = -2,
                    ErrorMessage = "Duplicate Id; Skipping."
                }));

            HttpResponseMessage response;
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK,
                                                  new ResultContainer
                                                      <MultipleGeocdeAddressResultContainer>
                                                      {
                                                          Status = (int) HttpStatusCode.OK,
                                                          Message = notifications,
                                                          Result = result
                                                      })
                                  .AddCache()
                                  .AddTypeHeader(typeof (ResultContainer<MultipleGeocdeAddressResultContainer>));
            }
            catch (InvalidOperationException)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadGateway) {
                    Content = new ObjectContent(typeof(ResultContainer), new ResultContainer
                    {
                        Message = "JSONP does not work with POST requests.",
                        Status = 400
                    }, new JsonMediaTypeFormatter())};
            }
            return response;
        }

        [HttpGet]
        public HttpResponseMessage Reverse(double? x, double? y, [FromUri] ReverseGeocodeOptions options)
        {
            #region validation

            var errors = "";
            if (!x.HasValue)
            {
                errors = "X is empty.";
            }

            if (!y.HasValue)
            {
                errors += "Y is emtpy";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<ReverseGeocodeResult>
                                                  {
                                                      Status = (int) HttpStatusCode.BadRequest,
                                                      Message = errors
                                                  });
            }

            #endregion

            var reverseGeocodeResponse =
                CommandExecutor.ExecuteCommand(new ReverseGeocodeCommand(new Location {X = x.Value, Y = y.Value},
                                                                         options.WkId, options.Distance));


            return Request.CreateResponse(HttpStatusCode.OK,
                                                        new ResultContainer<ReverseGeocodeResult>
                                                            {
                                                                Status = (int) HttpStatusCode.OK,
                                                                Result = reverseGeocodeResponse
                                                            })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<ReverseGeocodeResult>));
        }

        [HttpGet]
        public HttpResponseMessage RouteMilePost(string route, string milepost, [FromUri] MilepostOptions options)
        {
            double milepostNumber = -1;

            #region validation

            var errors = "";

            var standardRoute = CommandExecutor.ExecuteCommand(new StandardizeRouteNameCommand(route));

            if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(standardRoute))
            {
                errors = "route is empty or in an unexpected format. Expected number eg: 15, 89.";
            }

            if (string.IsNullOrEmpty(milepost) || !double.TryParse(milepost, out milepostNumber))
            {
                errors += "milepost is emtpy or in unexpected format. Expected number eg: 300, 300.009";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<ReverseGeocodeResult>
                                                  {
                                                      Status = (int) HttpStatusCode.BadRequest,
                                                      Message = errors
                                                  });
            }

            #endregion

            var response =
                CommandExecutor.ExecuteCommand(new MilepostCommand(standardRoute, milepostNumber, options));

            if (string.IsNullOrEmpty(response.Source))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<RouteMilepostResult>
                                                  {
                                                      Status = (int) HttpStatusCode.NotFound,
                                                      Message = $"route {route} and milepost {milepost} was not found."
                                                  })
                              .AddTypeHeader(typeof (ResultContainer<RouteMilepostResult>))
                              .AddCache();
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                                                        new ResultContainer<RouteMilepostResult>
                                                            {
                                                                Status = (int) HttpStatusCode.OK,
                                                                Result = response
                                                            })
                                .AddTypeHeader(typeof (ResultContainer<RouteMilepostResult>))
                              .AddCache();
        }

        [HttpGet]
        public async Task<HttpResponseMessage> ReverseMilePost(double? x, double? y,
                                                               [FromUri] ReverseMilepostOptions options)
        {
            #region validation

            var errors = "";
            if (!x.HasValue)
            {
                errors = "X is empty.";
            }

            if (!y.HasValue)
            {
                errors += "Y is emtpy";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<ReverseGeocodeResult>
                                                  {
                                                      Status = (int) HttpStatusCode.BadRequest,
                                                      Message = errors
                                                  });
            }

            #endregion

            var queryArgs = new ReverseMilepostArgs(x.Value, y.Value, options);

            var client = new HttpClient();

            var requestUri = ConfigurationManager.AppSettings["reverse_milepost_url"]
                .With(queryArgs.ToQueryString());

            HttpResponseMessage request;

            try
            {
                request = await client.GetAsync(requestUri);
            }
            catch (AggregateException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer
                                                  {
                                                      Status = (int) HttpStatusCode.InternalServerError,
                                                      Message = "I'm sorry, it seems as though the request had issues."
                                                  })
                               .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
            }

            try
            {
                request.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer
                                                  {
                                                      Status = (int) HttpStatusCode.InternalServerError,
                                                      Message = "I'm sorry, we were unable to communicate with the SOE."
                                                  })
                             .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
            }

            var response = await request.Content.ReadAsAsync<TopAndEqualMilepostCandidates>(new[]
                {
                    new TextPlainResponseFormatter()
                });


            if (response.TopResult == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ResultContainer
                    {
                        Status = (int) HttpStatusCode.NotFound,
                        Message = "No milepost was found within your buffer radius."
                    })
                              .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>))
                              .AddCache();
            }

            var transformed = new ReverseMilepostResult
                {
                    Route = response.TopResult.Route,
                    OffsetMeters = response.TopResult.Distance,
                    Milepost = response.TopResult.Milepost,
                    Increasing = response.TopResult.Increasing
                };

            return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<ReverseMilepostResult>
                {
                    Status = (int) HttpStatusCode.OK,
                    Result = transformed
                })
                          .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>))
                          .AddCache();
        }


        [HttpGet]
        public HttpResponseMessage ArcGisOnlineActivation(string callback)
        {
            const string agsResponseContent = @"{""currentVersion"":10.31,""serviceDescription"":"""",""addressFields"":[{""name"":""Street"",""type"":""esriFieldTypeString"",""alias"":""Street or Intersection"",""required"":true,""length"":100},{""name"":""City"",""type"":""esriFieldTypeString"",""alias"":""City or Placename"",""required"":false,""length"":40}],""singleLineAddressField"":{""name"":""Single Line Input"",""type"":""esriFieldTypeString"",""alias"":""Full Address"",""required"":false,""length"":100},""candidateFields"":[{""name"":""Shape"",""type"":""esriFieldTypeGeometry"",""alias"":""Shape"",""required"":false},{""name"":""Score"",""type"":""esriFieldTypeDouble"",""alias"":""Score"",""required"":false},{""name"":""Match_addr"",""type"":""esriFieldTypeString"",""alias"":""Match_addr"",""required"":false,""length"":120},{""name"":""House"",""type"":""esriFieldTypeString"",""alias"":""House"",""required"":false,""length"":12},{""name"":""Side"",""type"":""esriFieldTypeString"",""alias"":""Side"",""required"":false,""length"":1},{""name"":""PreDir"",""type"":""esriFieldTypeString"",""alias"":""PreDir"",""required"":false,""length"":6},{""name"":""PreType"",""type"":""esriFieldTypeString"",""alias"":""PreType"",""required"":false,""length"":6},{""name"":""StreetName"",""type"":""esriFieldTypeString"",""alias"":""StreetName"",""required"":false,""length"":32},{""name"":""SufType"",""type"":""esriFieldTypeString"",""alias"":""SufType"",""required"":false,""length"":6},{""name"":""SufDir"",""type"":""esriFieldTypeString"",""alias"":""SufDir"",""required"":false,""length"":6},{""name"":""City"",""type"":""esriFieldTypeString"",""alias"":""City"",""required"":false,""length"":20},{""name"":""State"",""type"":""esriFieldTypeString"",""alias"":""State"",""required"":false,""length"":2},{""name"":""ZIP"",""type"":""esriFieldTypeString"",""alias"":""ZIP"",""required"":false,""length"":5},{""name"":""X"",""type"":""esriFieldTypeDouble"",""alias"":""X"",""required"":false},{""name"":""Y"",""type"":""esriFieldTypeDouble"",""alias"":""Y"",""required"":false},{""name"":""User_fld"",""type"":""esriFieldTypeString"",""alias"":""User_fld"",""required"":false,""length"":120},{""name"":""Addr_type"",""type"":""esriFieldTypeString"",""alias"":""Addr_type"",""required"":false,""length"":20}],""spatialReference"":{""wkid"":26912,""latestWkid"":26912},""locatorProperties"":{""MinimumCandidateScore"":""60"",""SideOffsetUnits"":""Meters"",""UICLSID"":""{AE5A3A0E-F756-11D2-9F4F-00C04F8ED1C4}"",""SpellingSensitivity"":""80"",""MinimumMatchScore"":""60"",""EndOffset"":""5"",""MatchIfScoresTie"":""true"",""SideOffset"":""15"",""SuggestedBatchSize"":1000,""MaxBatchSize"":1000,""LoadBalancerTimeOut"":60,""WriteXYCoordFields"":""true"",""WriteStandardizedAddressField"":""0"",""WriteReferenceIDField"":""false"",""WritePercentAlongField"":""false""},""capabilities"":""Geocode,ReverseGeocode""}";
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(callback + "(" + agsResponseContent + ")", System.Text.Encoding.UTF8, "text/plain")
            };

            return resp;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> ArcGisOnline([FromUri] AgoGeocodeOptions options)
        {
            #region validation

            var errors = "";
            if (string.IsNullOrEmpty(options.Address))
            {
                errors = "Address is empty.";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                               new ResultContainer<GeocodeAddressResult>
                                               {
                                                   Status = (int)HttpStatusCode.BadRequest,
                                                   Message = errors
                                               });
            }

            var singleLineAddress = options.Address.Trim();

            #endregion

            var address = CommandExecutor.ExecuteCommand(new ParseSingleLineInputCommand(options.Address));

            if (string.IsNullOrEmpty(address.Zone))
            {
                errors += "Zip code or city name could not be extracted.";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<GeocodeAddressResult>
                                              {
                                                  Status = (int)HttpStatusCode.BadRequest,
                                                  Message = errors
                                              });
            }

            var wkidJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(options.WkId);

            var spatialReference = wkidJson["wkid"];
            if (wkidJson.ContainsKey("latestWkid"))
            {
                spatialReference = wkidJson["latestWkid"];
            }

            int wkid;
            int.TryParse(string.IsNullOrEmpty(spatialReference) ? "26912" : spatialReference, out wkid);

            var response = Get(address.Street, address.Zone, new GeocodeOptions
            {
                SuggestCount = options.SuggestCount,
                WkId = wkid
            });

            var container = await response.Content.ReadAsAsync<ResultContainer<GeocodeAddressResult>>();

            if (container.Status == 404)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new StringContent(@"{""candidates"":[]}"));
            }

            var esriItems = new List<Candidate>
            {
                new Candidate
                {
                    Address = container.Result.InputAddress,
                    Location = new Location(container.Result.Location.X, container.Result.Location.Y),
                    Score = container.Result.Score
                }
            };

            esriItems.AddRange(container.Result.Candidates);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                candidates = esriItems.OrderByDescending(x => x.Score)
            });
        }
    }
}
