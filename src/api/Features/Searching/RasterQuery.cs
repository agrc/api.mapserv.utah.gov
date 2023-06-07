// using System.Net;
// using System.Net.Http;
// using AGRC.api.Infrastructure;
// using AGRC.api.Models.ArcGis;
// using AGRC.api.Models.ResponseContracts;
// using Microsoft.AspNetCore.Mvc;

// namespace AGRC.api.Features.Searching;
// public class RasterElevation {
//     public class Computation : IComputation<ObjectResult> {
//         internal const string BaseUrl = "https://us-central1-ut-dts-agrc-web-api-prod.cloudfunctions.net/tls-downgrade";
//         internal readonly int[] LocalProjection = new[] { 3857, 10200 };

//         public Computation(string returnValues, SearchRequestOptionsContract options) {
//             Options = options;
//             ReturnValues = returnValues;
//         }

//         internal SearchRequestOptionsContract Options { get; }
//         internal string ReturnValues { get; }
//     }

//     public class Handler : IComputationHandler<Computation, ObjectResult> {
//         public async Task<ObjectResult> Handle(Computation request, CancellationToken cancellationToken) {
//             if (string.IsNullOrEmpty(request.Options.Geometry)) {
//                 return new OkObjectResult(new ApiResponseContract<IReadOnlyCollection<SearchResponseContract?>> {
//                     Result = Array.Empty<SearchResponseContract>(),
//                     Status = (int)HttpStatusCode.OK
//                 });
//             }

//             var identify = new Identify.RequestContract { GeometryType = GeometryType.esriGeometryPoint };
//             var point = CommandExecutor.ExecuteCommand(new DecodeInputGeomertryCommand(Geometry));

//             if (point is null && !string.IsNullOrEmpty(request.Options.Geometry)) {
//                 ErrorMessage = "GEOMETRY COORDINATES APPEAR TO BE INVALID.";
//                 return null;
//             }

//             if (point.SpatialReference is not null) {
//                 request.Options.SpatialReference = point.SpatialReference.Wkid;
//             }

//             HttpResponseMessage httpResponse;

//             if (!LocalProjection.Contains(Wkid)) {
//                 var coordinates = Enumerable.Empty<double>();
//                 if (point is null) {
//                     return null;
//                 }

//                 coordinates = new[] { point.X, point.Y };

//                 var projectResponse = CommandExecutor.ExecuteCommand(
//                     new ReprojectPointsCommand(
//                         new ReprojectPointsCommand.PointProjectQueryArgs(Wkid, 3857, coordinates.ToList())));

//                 if (!projectResponse.IsSuccessful) {
//                     return null;
//                 }

//                 identify.Geometry = string.Join("&", projectResponse.Geometries.Select(geo => $"{geo.X},{geo.Y}"));
//             } else {
//                 identify.Geometry = $"{point.X},{point.Y}";
//             }

//             var requestUri = $"{request.BaseUrl}{identify}";

//             try {
//                 httpResponse = await App.HttpClient.GetAsync(requestUri);
//             } catch (Exception ex) {
//                 ErrorMessage = ex.Message;

//                 return null;
//             }

//             Identify.ResponseContract response = null;
//             try {
//                 response = await httpResponse.Content.ReadAsAsync<Identify.ResponseContract>();
//             } catch (Exception ex) {
//                 ErrorMessage = ex.Message;
//             }

//             var attributes = new Dictionary<string, object>();
//             var values = request.ReturnValues.Split(',').Select(x => x.ToLowerInvariant());

//             if (values.Contains("feet")) {
//                 attributes["feet"] = response.Feet;
//             }

//             if (values.Contains("value")) {
//                 attributes["value"] = response.Value;
//             }

//             if (values.Contains("meters")) {
//                 attributes["meters"] = response.Value;
//             }

//             return new SearchResult {
//                 Attributes = attributes
//             };
//         }
//     }
// }
