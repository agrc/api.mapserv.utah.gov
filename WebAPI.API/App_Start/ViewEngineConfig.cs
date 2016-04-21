using System.Web.Mvc;

namespace WebAPI.API
{
    public class ViewEngineConfig
    {
        public static void RegisterViewEngine(IViewEngine item)
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(item);
        }
    }
}