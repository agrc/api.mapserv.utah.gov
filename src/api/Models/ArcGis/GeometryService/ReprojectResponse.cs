namespace AGRC.api.Models.ArcGis;
public record ReprojectResponse<T>(IReadOnlyCollection<T> Geometries, RestEndpointError? Error) : RestErrorable(Error);
