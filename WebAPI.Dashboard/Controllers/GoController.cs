using System.Web.Mvc;
using Ninject;
using WebAPI.Common.Providers;

namespace WebAPI.Dashboard.Controllers
{
    public class GoController : Controller
    {
        [Inject]
        public IDomainLinkProvider DomainLinkProvider { get; set; }

        [HttpGet]
        public RedirectResult Api()
        {
            return Redirect(DomainLinkProvider.ApiLink);
        }
    }
}
