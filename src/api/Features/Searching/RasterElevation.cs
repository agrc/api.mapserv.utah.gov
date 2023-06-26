using System.Net.Http;
using System.Net.Http.Formatting;
using AGRC.api.Formatters;
using AGRC.api.Infrastructure;
using AGRC.api.Models.ArcGis;

namespace AGRC.api.Features.Searching;
public class RasterElevation {
    public class Computation(string returnValues, SearchOptions options) : IComputation<IReadOnlyCollection<SearchResponseContract?>?> {
        internal SearchOptions Options { get; } = options;
        internal string ReturnValues { get; } = returnValues;
    }

    public class Handler(IHttpClientFactory httpClientFactory, ILogger log) : IComputationHandler<Computation, IReadOnlyCollection<SearchResponseContract?>?> {
        internal readonly HttpClient _client = httpClientFactory.CreateClient("national-map");
        internal readonly MediaTypeFormatter[] _mediaTypes = new MediaTypeFormatter[] {
                new TextPlainResponseFormatter()
            };
        internal readonly ILogger? _log = log?.ForContext<RasterElevation>();

        public async Task<IReadOnlyCollection<SearchResponseContract?>?> Handle(Computation computation, CancellationToken cancellationToken) {
            if (computation.Options.Point is null) {
                _log?.ForContext("geometry", computation.Options.Geometry)
                   .Warning("invalid elevation query geometry");

                throw new ArgumentException("The input geometry appears to be invalid.");
            }

            var elevationIdentify = new ImageServiceIdentify.RequestContract(computation.Options.Point, GeometryType.esriGeometryPoint);
            var requestUri = elevationIdentify.ToQuery();

            _log?.ForContext("url", requestUri)
                 .Debug("request generated");

            HttpResponseMessage httpResponse;
            try {
                httpResponse = await _client.GetAsync(requestUri, cancellationToken);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "failed");

                throw new TaskCanceledException("The request was canceled.");
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "request error");

                throw new HttpRequestException("There was a problem handling your request.");
            }

            ImageServiceIdentify.ResponseContract? response;
            try {
                response = await httpResponse.Content.ReadAsAsync<ImageServiceIdentify.ResponseContract>(_mediaTypes, cancellationToken);
            } catch (Exception ex) {
                _log?.ForContext("url", requestUri)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "error reading response");

                throw new Exception("There was an unexpected response from UDOT.");
            }

            if (!(response?.IsSuccessful ?? false)) {
                _log?.ForContext("request", requestUri)
                    .ForContext("error", response?.Error)
                    .Warning("invalid request");

                throw new ArgumentException("Your request was invalid. Check that your coordinates and spatial reference match.");
            }

            var attributes = new Dictionary<string, object>();
            var values = computation.ReturnValues.Split(',').Select(x => x.ToLowerInvariant().Trim());

            if (values.Contains("feet")) {
                attributes["feet"] = response.Feet;
            }

            if (values.Contains("value")) {
                attributes["value"] = response.Value;
            }

            if (values.Contains("meters")) {
                attributes["meters"] = response.Value;
            }

            return new[]{
                new SearchResponseContract {
                    Attributes = attributes
                },
            };
        }
    }
}
