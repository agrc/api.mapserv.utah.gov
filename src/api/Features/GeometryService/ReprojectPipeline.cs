using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using api.mapserv.utah.gov.Features.Geocoding;
using api.mapserv.utah.gov.Models;
using api.mapserv.utah.gov.Models.ArcGis;
using MediatR;
using Serilog;

namespace api.mapserv.utah.gov.Features.GeometryService {
    public class ReprojectPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TResponse : Candidate {
        private readonly IMediator _mediator;
        private readonly ILogger _log;

        public ReprojectPipeline(IMediator mediator, ILogger log)
        {
            _mediator = mediator;
            _log = log?.ForContext<ReprojectPipeline<TRequest, TResponse>>();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
            var response = await next();
            PointReprojectOptions options = null;

            if (response == null) {
                return response;
            }

            if (request is IHasGeocodingOptions options1) {
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

            var projected = await _mediator.Send(new Reproject.Command(options));

            if (!projected.IsSuccessful || !projected.Geometries.Any()) {
                _log.Fatal("Could not reproject point for {candidate}", response);

                return response;
            }

            var points = projected.Geometries.First();

            response.Location = new Point(points.X, points.Y);

            return response;
        }
    }
}
