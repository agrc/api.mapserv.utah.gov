namespace AGRC.api.Models.ArcGis;
public abstract record RestErrorable(RestEndpointError? Error) {
    public virtual bool IsSuccessful => Error == null;
}
