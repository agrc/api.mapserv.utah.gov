namespace WebAPI.Common.Providers.Urls
{
    public class LocalDomainLinkProvider : IDomainLinkProvider
    {
        private string _apiLink = "http://webapi";
        private string _dashboardLink = "http://dashboard";

        public string ApiLink
        {
            get { return _apiLink; }
            set { _apiLink = value; }
        }

        public string DashboardLink
        {
            get { return _dashboardLink; }
            set { _dashboardLink = value; }
        }
    }

    public class ProductionDomainLinkProvider : IDomainLinkProvider
    {
        private string _apiLink = "//api.mapserv.utah.gov";
        private string _dashboardLink = "//developer.mapserv.utah.gov";

        public string ApiLink
        {
            get { return _apiLink; }
            set { _apiLink = value; }
        }

        public string DashboardLink
        {
            get { return _dashboardLink; }
            set { _dashboardLink = value; }
        }
    }
}
