using System.Web.Optimization;

namespace WebAPI.Dashboard
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-1.10.1.js"));


            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                "~/Scripts/osDetect.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap",
                                         "//netdna.bootstrapcdn.com/twitter-bootstrap/2.3.1/js/bootstrap.min.js").
                            Include(
                                "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/bundles").Include(
                "~/Content/bootstrap.css",
                "~/Content/Site.css"));
        }
    }
}