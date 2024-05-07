namespace ugrc.api.Models.ArcGis;
public record ServiceInformation(string ServiceDescription, RestEndpointError? Error) : RestErrorable(Error);
