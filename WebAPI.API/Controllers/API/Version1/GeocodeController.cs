using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Serilog;
using WebAPI.API.Commands.Geocode;
using WebAPI.Common.Exceptions;
using WebAPI.Common.Executors;
using WebAPI.Common.Extensions;
using WebAPI.Common.Formatters;
using WebAPI.Common.Models.Esri.RoadsAndHighways;
using WebAPI.Domain;
using WebAPI.Domain.ApiResponses;
using WebAPI.Domain.ArcServerResponse.Geolocator;
using WebAPI.Domain.EndpointArgs;
using WebAPI.Domain.InputOptions;

namespace WebAPI.API.Controllers.API.Version1
{
    public class GeocodeController : ApiController
    {
        private const int MaxAddresses = 100;

        [HttpGet]
        public HttpResponseMessage Get(string street, string zone, [FromUri] GeocodeOptions options)
        {
            var log = Log.ForContext<GeocodeController>();
            var geocodeLog = log.ForContext("request-id", $"{street},{zone}");

            geocodeLog.Warning("geocode(single): street {street}, zone {zone}, with {@options}", street, zone, options);

            #region validation

            street = street?.Trim();
            zone = zone?.Trim();

            zone = zone?.Replace(".", "").Replace(",", "").Replace("_", " ");

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
                                                  Status = (int)HttpStatusCode.BadRequest,
                                                  Message = errors
                                              });
            }

            #endregion

            GeocodeAddressResult geocodeAddressResult;

            try
            {
                geocodeAddressResult =
                    CommandExecutor.ExecuteCommand(new OrchesrateeGeocodeCommand(street, zone, options));
            }
            catch (AggregateException ex)
            {
                geocodeLog.Fatal(ex, "geocode(single): Aggregate error from geocoding");
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                        new ResultContainer<GeocodeAddressResult>
                        {
                            Status = (int)HttpStatusCode.InternalServerError,
                            Message = "Geocoding error occured.",
                            Result = new GeocodeAddressResult
                            {
                                InputAddress = $"{street}, {zone}",
                                Score = -9
                            }
                        })
                    .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));
            }
            catch (GeocodingException ex)
            {
                geocodeLog.Fatal(ex, "geocode(single): Geocoding error from geocoding");
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                            new ResultContainer<GeocodeAddressResult>
                            {
                                Status = (int)HttpStatusCode.InternalServerError,
                                Message = $"Geocoding error occured. {ex.Message}",
                                Result = new GeocodeAddressResult
                                {
                                    InputAddress = $"{street}, {zone}",
                                    Score = -9
                                }
                            })
                        .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));
            }
            catch (Exception ex)
            {
                geocodeLog.Fatal(ex, "geocode(single): Normal error from geocoding");
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                        new ResultContainer<GeocodeAddressResult>
                        {
                            Status = (int)HttpStatusCode.InternalServerError,
                            Message = $"Geocoding error occured.",
                            Result = new GeocodeAddressResult
                            {
                                InputAddress = $"{street}, {zone}",
                                Score = -9
                            }
                        })
                    .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));
            }

            if (geocodeAddressResult == null || geocodeAddressResult.Score < 0)
            {
                geocodeLog.Warning("geocode(single): Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
                         options.AcceptScore);

                return Request.CreateResponse(HttpStatusCode.NotFound,
                                                            new ResultContainer<GeocodeAddressResult>
                                                            {
                                                                Status = (int)HttpStatusCode.NotFound,
                                                                Message = $"No address candidates found with a score of {options.AcceptScore} or better."
                                                            })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));
            }

            if (geocodeAddressResult.Location == null)
            {
                geocodeLog.Warning("geocode(single): Could not find match for {Street}, {Zone} with a score of {Score} or better.", street, zone,
                         options.AcceptScore);
            }

            geocodeAddressResult.Wkid = options.WkId;

            geocodeLog.Warning("geocode(single): success score {score}, {@result}", geocodeAddressResult.Score, geocodeAddressResult);

            var response = Request.CreateResponse(HttpStatusCode.OK,
                                                                new ResultContainer<GeocodeAddressResult>
                                                                {
                                                                    Status = (int)HttpStatusCode.OK,
                                                                    Result = geocodeAddressResult
                                                                })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<GeocodeAddressResult>));

            return response;
        }

        [HttpPost]
        public HttpResponseMessage Multiple(MultipleGeocodeContainerArgs addresseses, [FromUri] GeocodeOptions options)
        {
            var log = Log.ForContext<GeocodeController>();
            log.Warning("geocode(multiple): options {@options}", options);

            var notifications = "";

            #region validation

            if (addresseses == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                                            new ResultContainer<MultipleGeocdeAddressResultContainer>
                                                            {
                                                                Status = (int)HttpStatusCode.BadRequest,
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
                                                                Status = (int)HttpStatusCode.BadRequest,
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
                log.Warning("success(multiple): success count {count}, average score {score}", result.Addresses.Count(), result.Addresses.Select(x => x.Score).Distinct());
                response = Request.CreateResponse(HttpStatusCode.OK,
                                                  new ResultContainer
                                                      <MultipleGeocdeAddressResultContainer>
                                                  {
                                                      Status = (int)HttpStatusCode.OK,
                                                      Message = notifications,
                                                      Result = result
                                                  })
                                  .AddCache()
                                  .AddTypeHeader(typeof(ResultContainer<MultipleGeocdeAddressResultContainer>));
            }
            catch (InvalidOperationException ex)
            {
                log.Fatal(ex, "geocode(multiple): invalid operation jsonp?");

                response = new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new ObjectContent(typeof(ResultContainer), new ResultContainer
                    {
                        Message = "JSONP does not work with POST requests.",
                        Status = 400
                    }, new JsonMediaTypeFormatter())
                };
            }

            return response;
        }

        [HttpGet]
        public HttpResponseMessage Reverse(double? x, double? y, [FromUri] ReverseGeocodeOptions options)
        {
            var log = Log.ForContext<GeocodeController>();
            var reverseLog = log.ForContext("request-id", $"{x},{y}");

            reverseLog.Warning("geocode(reverse): {x},{y}, with {@options}", x, y, options);

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
                                                  Status = (int)HttpStatusCode.BadRequest,
                                                  Message = errors
                                              });
            }

            #endregion

            var reverseGeocodeResponse =
                CommandExecutor.ExecuteCommand(new ReverseGeocodeCommand(new Location { X = x.Value, Y = y.Value },
                                                                         options.WkId, options.Distance));

            reverseLog.Warning("geocode(reverse): success {@result}", reverseGeocodeResponse);

            return Request.CreateResponse(HttpStatusCode.OK,
                                                        new ResultContainer<ReverseGeocodeResult>
                                                        {
                                                            Status = (int)HttpStatusCode.OK,
                                                            Result = reverseGeocodeResponse
                                                        })
                            .AddCache()
                            .AddTypeHeader(typeof(ResultContainer<ReverseGeocodeResult>));
        }

        [HttpGet]
        public async Task<HttpResponseMessage> RouteMilePost(string route, string milepost, [FromUri] MilepostOptions options)
        {
            var log = Log.ForContext<GeocodeController>();
            var specificLog = log.ForContext("request-id", $"{route},{milepost}");

            specificLog.Warning("geocode(milepost): {route},{milepost}, with {@options}", route, milepost, options);

            double milepostNumber = -1;

            #region validation

            var errors = "";

            var standardRoute = route;
            if (!options.FullRoute)
            {
                standardRoute = CommandExecutor.ExecuteCommand(new StandardizeRouteNameCommand(route, options.Side));
            }

            if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(standardRoute))
            {
                errors = "route is empty or in an unexpected format. Expected number eg: 15, 89.";
            }

            if (string.IsNullOrEmpty(milepost) || !double.TryParse(milepost, out milepostNumber))
            {
                errors += "milepost is empty or in unexpected format. Expected number eg: 300, 300.009";
            }

            if (errors.Length > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<ReverseGeocodeResult>
                                              {
                                                  Status = (int)HttpStatusCode.BadRequest,
                                                  Message = errors
                                              });
            }

            #endregion

            var response = await
                CommandExecutor.ExecuteCommandAsync(new MilepostCommand(standardRoute, milepostNumber, options));

            if (response is null)
            {
                specificLog.Warning("geocode(milepost): not found");

                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              new ResultContainer<RouteMilepostResult>
                                              {
                                                  Status = (int)HttpStatusCode.NotFound,
                                                  Message = $"route {route} and milepost {milepost} was not found."
                                              })
                              .AddTypeHeader(typeof(ResultContainer<RouteMilepostResult>))
                              .AddCache();
            }

            specificLog.Warning("geocode(milepost): success {@response}", response);

            return Request.CreateResponse(HttpStatusCode.OK,
                                                        new ResultContainer<RouteMilepostResult>
                                                        {
                                                            Status = (int)HttpStatusCode.OK,
                                                            Result = response
                                                        })
                                .AddTypeHeader(typeof(ResultContainer<RouteMilepostResult>))
                              .AddCache();
        }

        [HttpGet]
        public async Task<HttpResponseMessage> ReverseMilePost(double? x, double? y,
                                                               [FromUri] ReverseMilepostOptions options)
        {
            var log = Log.ForContext<GeocodeController>();
            var specificLog = log.ForContext("request-id", $"{x},{y}");

            specificLog.Warning("geocode(reverse-milepost): {x},{y}, with {@options}", x, y, options);

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
                                                  Status = (int)HttpStatusCode.BadRequest,
                                                  Message = errors
                                              });
            }

            #endregion

            var baseUrl = "https://roads.udot.utah.gov/server/rest/services/LrsEnabled/Read_Only_Public_LRS_Routes/MapServer/exts/LRServer/networkLayers/1/";

            var point = new GeometryToMeasure.Point(x.Value, y.Value);
            var requestContract = new GeometryToMeasure.RequestContract
            {
                Locations = new[] {
                        new GeometryToMeasure.RequestLocation { Geometry = point }
                    },
                OutSr = options.WkId,
                InSr = options.WkId,
                Tolerance = options.Buffer
            };

            var requestUri = $"{baseUrl}geometryToMeasure{requestContract.QueryString}";

            HttpResponseMessage httpResponse;

            try
            {
                httpResponse = await App.HttpClient.GetAsync(requestUri);
            }
            catch (AggregateException ex)
            {
                specificLog.Fatal(ex, "geocode(reverse - milepost): aggregate exception");

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer
                                              {
                                                  Status = (int)HttpStatusCode.InternalServerError,
                                                  Message = "I'm sorry, it seems as though the request had issues."
                                              })
                               .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
            }

            try
            {
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                specificLog.Fatal(ex, "geocode(reverse-milepost): general exception");

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer
                                              {
                                                  Status = (int)HttpStatusCode.InternalServerError,
                                                  Message = "I'm sorry, we were unable to communicate with the UDOT service."
                                              })
                             .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
            }

            GeometryToMeasure.ResponseContract response;

            try
            {
                response = await httpResponse.Content.ReadAsAsync<GeometryToMeasure.ResponseContract>(new[]
                {
                    new TextPlainResponseFormatter()
                });
            }
            catch (Exception ex)
            {
                specificLog.Fatal(ex, "geocode(reverse-milepost): error reading result");

                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                                              new ResultContainer
                                              {
                                                  Status = (int)HttpStatusCode.InternalServerError,
                                                  Message = "I'm sorry, we were unable to read the result."
                                              })
                             .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
            }

            if (!response.IsSuccessful)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                             new ResultContainer
                                             {
                                                 Status = (int)HttpStatusCode.BadRequest,
                                                 Message = "Your request was invalid. Check that your coordinates and spatial reference match."
                                             })
                            .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
            }

            if (response.Locations?.Length != 1)
            {
                // this should not happen
                specificLog.Warning("geocode(reverse-milepost): multiple locations found");
            }

            var location = response.Locations[0];

            if (location.Status != GeometryToMeasure.Status.esriLocatingOK)
            {
                if (location.Status != GeometryToMeasure.Status.esriLocatingMultipleLocation)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                                             new ResultContainer
                                             {
                                                 Message = "No milepost was found within your buffer radius.",
                                                 Status = (int)HttpStatusCode.NotFound
                                             })
                         .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
                }

                // concurrency
                var primaryRoutes = FilterPrimaryRoutes(location.Results, options.IncludeRampSystems == 1);

                if (primaryRoutes.Count < 1)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound,
                                             new ResultContainer
                                             {
                                                 Message = "No milepost was found within your buffer radius.",
                                                 Status = (int)HttpStatusCode.NotFound
                                             })
                         .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>));
                }

                var dominantRoutes = await CommandExecutor.ExecuteCommandAsync(
                    new DominantRouteResolverCommand(primaryRoutes, point, options.SuggestCount));

                return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<ReverseMilepostResult>
                {
                    Status = (int)HttpStatusCode.OK,
                    Result = dominantRoutes
                })
                          .AddTypeHeader(typeof(ResultContainer<ReverseMilepostResult>))
                          .AddCache();
            }

            var routes = FilterPrimaryRoutes(location.Results, options.IncludeRampSystems == 1);

            if (routes.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new ResultContainer<ReverseMilepostResult>
                {
                    Message = "No milepost was found within your buffer radius.",
                    Status = (int)HttpStatusCode.NotFound
                });
            }

            location = routes[0];

            return Request.CreateResponse(HttpStatusCode.OK, new ResultContainer<ReverseMilepostResult>
            {
                Result = new ReverseMilepostResult
                {
                    Route = location.RouteId,
                    OffsetMeters = 0,
                    Milepost = location.Measure,
                    Dominant = true,
                    Candidates = Array.Empty<ReverseMilepostResult>()
                },
                Status = (int)HttpStatusCode.OK
            });
        }

        private IList<GeometryToMeasure.ResponseLocation> FilterPrimaryRoutes(
            GeometryToMeasure.ResponseLocation[] locations, bool includeRamps)
        {
            var udotRoutes = new Regex($"[P|N][M|C{(includeRamps ? "|R" : "")}]", RegexOptions.IgnoreCase,
                    TimeSpan.FromSeconds(2));
            var filtered = new List<GeometryToMeasure.ResponseLocation>(locations.Length);

            for (var i = 0; i < locations.Length; i++)
            {
                var location = locations[i];
                var routeId = location.RouteId;

                // only allow udot routes with positive or negative values
                // and then only allow mainline collectors and optionally ramps
                // this will filter out surface streets e.g., 12TVL23541500_600_N
                // but keep aid routes etc
                if (!udotRoutes.IsMatch(routeId.Substring(4, 2)))
                {
                    continue;
                }

                filtered.Add(location);
            }

            return filtered;
        }
    }
}
