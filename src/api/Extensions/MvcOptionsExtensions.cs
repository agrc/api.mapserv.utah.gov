using api.mapserv.utah.gov.Filters;
using Microsoft.AspNetCore.Mvc;

namespace api.mapserv.utah.gov.Extensions {
    public static class MvcOptionsExtensions {
        public static void AddApiResponseFormatters(this MvcOptions mvcOptions) =>
            mvcOptions.Filters.Add(typeof(JsonOutputFormatResultFilter));

        public static void AddResponseCache(this MvcOptions mvcOptions) {
            mvcOptions.Filters.Add(typeof(CacheResourceFilter));
        }

        public static void AddResultCache(this MvcOptions mvcOptions) {
            mvcOptions.Filters.Add(typeof(CacheResultFilter));
        }
    }
}
