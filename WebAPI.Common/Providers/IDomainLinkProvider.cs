namespace WebAPI.Common.Providers
{
    public interface IDomainLinkProvider
    {
        string ApiLink { get; set; }
        string DashboardLink { get; set; }
    }
}