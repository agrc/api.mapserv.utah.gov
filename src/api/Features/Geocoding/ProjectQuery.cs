using System.Net.Http;
using System.Net.Http.Formatting;
using Microsoft.Extensions.Options;
using ugrc.api.Formatters;
using ugrc.api.Infrastructure;
using ugrc.api.Models;
using ugrc.api.Models.ArcGis;

namespace ugrc.api.Features.Geocoding;
public class ProjectQuery {
    public record PointReprojectResponse(string ServiceDescription, RestEndpointError? Error) : RestErrorable(Error);
    public record PointReprojectInput(List<Point> Geometries);

    public class Computation(Candidate? candidate, int wkid) : IComputation<Candidate?> {
        public Candidate? Candidate { get; } = candidate;
        public int Wkid { get; } = wkid;
    }

    public class Handler(IHttpClientFactory httpClientFactory, IOptions<GeometryServiceConfiguration> options, ILogger log) : IComputationHandler<Computation, Candidate?> {
        private readonly HttpClient _client = httpClientFactory.CreateClient("arcgis");
        private readonly MediaTypeFormatter[] _mediaTypes = [new TextPlainResponseFormatter()];
        private readonly GeometryServiceConfiguration _geometryService = options.Value;
        private readonly ILogger? _log = log?.ForContext<GeocodeQuery>();

        public async Task<Candidate?> Handle(Computation request, CancellationToken cancellationToken) {
            if (request.Wkid == 26912) {
                return request.Candidate;
            }

            if (request.Candidate is null) {
                return request.Candidate;
            }

            var requestContract = new Project.RequestContract {
                OutSr = request.Wkid,
                Locations = [new Project.RequestGeometry(request.Candidate.Location)]
            };

            var requestUri = $"{_geometryService.Url()}{requestContract.QueryString}";

            _log?.ForContext("url", requestUri)
                .Debug("Request generated");

            HttpResponseMessage httpResponse;
            try {
                httpResponse = await _client.GetAsync(requestUri, cancellationToken);
            } catch (TaskCanceledException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "Geometry service project failed");

                return request.Candidate;
            } catch (HttpRequestException ex) {
                _log?.ForContext("url", requestUri)
                    .Fatal(ex, "Request error");

                return request.Candidate;
            }

            Project.ResponseContract response;

            try {
                response = await httpResponse.Content.ReadAsAsync<Project.ResponseContract>(_mediaTypes, cancellationToken);
                if (!response.IsSuccessful) {
                    _log?.ForContext("request", request)
                        .ForContext("error", response.Error)
                        .Warning("Invalid request");

                    return request.Candidate;
                }
            } catch (Exception ex) {
                _log?.ForContext("url", requestUri)
                    .ForContext("response", await httpResponse.Content.ReadAsStringAsync(cancellationToken))
                    .Fatal(ex, "Error reading response");

                return request.Candidate;
            }

            if (response.Geometries?.Count != 1) {
                // this should not happen
                _log?.ForContext("response", response)
                    .Warning("Multiple geometries found");
            }

            if (response.Geometries is null) {
                return request.Candidate;
            }

            var projectedPoint = response.Geometries.FirstOrDefault();

            var projectedCandidate = request.Candidate;
            projectedCandidate.Location = projectedPoint ?? request.Candidate.Location;

            return projectedCandidate;
        }
    }
}
