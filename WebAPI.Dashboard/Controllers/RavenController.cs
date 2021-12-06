using System.Linq;
using System.Web.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using WebAPI.Common.Indexes;
using WebAPI.Common.Models.Raven.Admin;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Dashboard.Controllers
{
    public abstract class RavenController : BaseController
    {
        protected RavenController(IDocumentStore store)
        {
            DocumentStore = store;
        }

        public new IDocumentSession Session { get; set; }

        public IDocumentStore DocumentStore { get; set; }

        protected Account Account
        {
            get
            {
                return Session.Query<Account, IndexEmail>()
                              .SingleOrDefault(x => x.Id == User.Identity.Name);
            }
        }

        [HttpGet, OutputCache(Duration = 3600 /*one hour*/, VaryByCustom = "App.OutputCacheKey")]
        public PartialViewResult RenderUserPanel()
        {
            return PartialView("LoginStatus", Account);
        }

        [HttpGet, OutputCache(Duration = 86400 /*one day*/, VaryByCustom = "App.User.Identity.Name")]
        public PartialViewResult RenderAdminAnalyticLink()
        {
            return Session.Query<AdminContainer>().Any(x => x.Emails.Any(y => y == Account.Email))
                       ? PartialView("AnalyticLink", Account)
                       : null;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session = DocumentStore.OpenSession();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (Session)
            {
                if (Session != null && filterContext.Exception == null && Session.Advanced.HasChanges)
                {
                    Session.SaveChanges();
                }
            }
        }
    }
}