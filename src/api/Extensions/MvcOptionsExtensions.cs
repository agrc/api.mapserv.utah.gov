using AGRC.api.Filters;
using Microsoft.AspNetCore.Mvc;

#nullable enable
namespace AGRC.api.Extensions;
public static class MvcOptionsExtensions {
    public static void AddApiResponseFormatters(this MvcOptions mvcOptions) =>
        mvcOptions.Filters.Add(typeof(JsonOutputFormatResultFilter));
}
