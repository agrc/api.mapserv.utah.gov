using System.Linq;
using System.Web.Mvc;

namespace WebAPI.Common.Extensions
{
    public static class ModselStateExtensions
    {
        public static string FlattenErrors(this ModelStateDictionary modelState)
        {
            return string.Join(" ", modelState.Values
                                              .Where(key => key.Errors.Any())
                                              .SelectMany(x => x.Errors
                                                                .Select(y => y.ErrorMessage)));
        }
    }
}