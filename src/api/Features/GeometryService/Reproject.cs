using System.Net.Http;
using System.Net.Http.Formatting;
using AGRC.api.Features.Geocoding;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models;
using AGRC.api.Models.ArcGis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AGRC.api.Features.GeometryService;
public class Reproject {
    public class Computation : IComputation<ReprojectResponse<Point>?> {
        public Computation(PointReprojectOptions options) {
            Options = options;
        }

        public readonly PointReprojectOptions Options;
        public string ReprojectUrl { get; set; } = string.Empty;
    }

    public class Handler : IComputationHandler<Computation, ReprojectResponse<Point>?> {
        private readonly HttpClient _client;
        private readonly IOptions<GeometryServiceConfiguration> _geometryServiceConfiguration;
        private readonly ILogger? _log;
        private readonly MediaTypeFormatter[] _mediaTypes;

        public Handler(IOptions<GeometryServiceConfiguration> geometryServiceConfiguration,
                       IHttpClientFactory clientFactory, ILogger log) {
            _geometryServiceConfiguration = geometryServiceConfiguration;
            _client = clientFactory.CreateClient("arcgis");
            _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };
            _log = log?.ForContext<Reproject>();
        }

        public async Task<ReprojectResponse<Point>?> Handle(Computation request, CancellationToken token) {
            if (string.IsNullOrEmpty(request.ReprojectUrl)) {
                request.ReprojectUrl = _geometryServiceConfiguration.Value.Url();
            }

            var query = new QueryString("?f=json");
            query = query.Add("inSR", request.Options.CurrentSpatialReference.ToString());
            query = query.Add("outSR", request.Options.ReprojectToSpatialReference.ToString());
            query = query.Add("geometries", string.Join(",", request.Options.Coordinates));

            var requestUri = string.Format(request.ReprojectUrl, query.Value);

            _log?.ForContext("url", request.ReprojectUrl)
                .Debug("request generated", request.Options, request.ReprojectUrl);

            ReprojectResponse<Point>? result = null;
            HttpResponseMessage response;

            try {
                response = await _client.GetAsync(requestUri, token);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", request.ReprojectUrl)
                    .Fatal(ex, "failed");

                return result;
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", request.ReprojectUrl)
                    .Fatal(ex, "request error");

                return result;
            }

            try {
                result = await response.Content.ReadAsAsync<ReprojectResponse<Point>>(_mediaTypes, token);
            } catch (Exception ex) {
                _log?.ForContext("url", request.ReprojectUrl)
                    .ForContext("response", await response.Content.ReadAsStringAsync(token))
                    .Fatal(ex, "error reading response");

                return result;
            }

            return result;
        }
    }

    public class Decorator<TComputation, TResponse>
        : IComputationHandler<TComputation, TResponse>
            where TComputation
        : IComputation<TResponse> where TResponse : Candidate? {
        private readonly IComputationHandler<TComputation, TResponse> _decorated;
        private readonly IComputeMediator _computeMediator;
        private readonly ILogger? _log;

        public Decorator(IComputationHandler<TComputation, TResponse> decorated, IComputeMediator computeMediator, ILogger log) {
            _decorated = decorated;
            _computeMediator = computeMediator;
            _log = log?.ForContext<Decorator<TComputation, TResponse>>();
        }

        public async Task<TResponse> Handle(TComputation computation, CancellationToken cancellationToken) {
            var response = await _decorated.Handle(computation, cancellationToken);
            PointReprojectOptions? options = null;

            if (response == null) {
                return response;
            }

            if (computation is IHasGeocodingOptions options1) {
                var geocodingOptions = options1.Options;

                if (geocodingOptions.SpatialReference == 26912) {
                    return response;
                }

                options = new PointReprojectOptions(26912, geocodingOptions.SpatialReference,
                                                                    new[] {
                                                                    response.Location.X,
                                                                    response.Location.Y
                                                                    });
            }

            if (options == null) {
                return response;
            }

            var projected = await _computeMediator.Handle(new Computation(options), cancellationToken);

            if (projected?.IsSuccessful != true || !projected.Geometries.Any()) {
                _log?.ForContext("candidate", string.Join(",", options.Coordinates))
                    .ForContext("inSR", options.CurrentSpatialReference)
                    .ForContext("outSR", options.ReprojectToSpatialReference)
                    .Fatal("failed");

                return response;
            }

            var points = projected.Geometries.First();

            response.Location = new Point(points.X, points.Y);

            return response;
        }
    }
}
