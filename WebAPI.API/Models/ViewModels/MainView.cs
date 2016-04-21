using System.Collections.Generic;
using System.Web.Mvc;

namespace WebAPI.API.Models.ViewModels
{
    public class MainView
    {
        public IEnumerable<SelectListItem> SgidCategories { get; set; }

        public IEnumerable<SelectListItem> SgidVersion { get; set; }

        internal MainView WithSgidCategories(IEnumerable<SelectListItem> items)
        {
            SgidCategories = items;
            return this;
        }

        internal MainView WithSgidVersions(IEnumerable<SelectListItem> items)
        {
            SgidVersion = items;
            return this;
        }
    }
}