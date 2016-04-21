using System.Web.Mvc;
using Ninject;
using WebAPI.Common.Providers;

namespace WebAPI.API.Controllers
{
    public class GoController : Controller
    {
        [Inject]
        public IDomainLinkProvider DomainLinkProvider { get; set; }

        [HttpGet]
        public RedirectResult Dashboard()
        {
            return Redirect(DomainLinkProvider.DashboardLink);
        }

        [HttpGet]
        public RedirectResult Register()
        {
            return Redirect(DomainLinkProvider.DashboardLink);
        }

        [HttpGet]
        public RedirectResult Startup()
        {
            return Redirect(DomainLinkProvider.DashboardLink + "/StartupGuide");
        }
    }
}