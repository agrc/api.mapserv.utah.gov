using AGRC.api.Features.Converting;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;
using AGRC.api.Models.ResponseContracts;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;

namespace AGRC.api.Features.Geocoding;

public class JsonOutputFormatDecorator(IRequestHandler<GeocodeQuery.Query, IApiResponse> decorated, IComputeMediator computeMediator, ApiVersion apiVersion) : IRequestHandler<GeocodeQuery.Query, IApiResponse> {

    public async Task<IApiResponse> Handle(GeocodeQuery.Query request, CancellationToken cancellationToken) {
        var result = await decorated.Handle(request, cancellationToken);

        if (result is ApiResponseContract<SingleGeocodeResponseContract> single) {
            if (single.Result is null || single.Status != StatusCodes.Status200OK || request._options.Format == JsonFormat.None) {
                return single;
            }

            if (request._options.Format == JsonFormat.EsriJson) {
                var command = new EsriGraphic.Computation(single.Result, apiVersion);
                var response = await computeMediator.Handle(command, default);

                return new ApiResponseContract<EsriGraphic.SerializableGraphic> {
                    Result = response,
                    Status = single?.Status ?? StatusCodes.Status500InternalServerError
                };
            } else if (request._options.Format == JsonFormat.GeoJson) {
                var command = new GeoJsonFeature.Computation(single.Result, apiVersion);
                var response = await computeMediator.Handle(command, default);

                return new ApiResponseContract<NetTopologySuite.Features.Feature> {
                    Result = response,
                    Status = single?.Status ?? StatusCodes.Status500InternalServerError
                };
            }
        }

        return result;
    }
}
