namespace ugrc.api.Models.ArcGis;

public record RestEndpointError(int Code, string? Message, IReadOnlyCollection<object>? Details);
