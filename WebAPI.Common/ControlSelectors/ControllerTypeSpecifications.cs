using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http.Dispatcher;

namespace WebAPI.Common.ControlSelectors
{
    public static class ControllerTypeSpecifications
    {
        public static IEnumerable<KeyValuePair<string, Type>> ByAreaName(
            this IEnumerable<KeyValuePair<string, Type>> query, string areaName)
        {
            var areaNameToFind = string.Format(CultureInfo.InvariantCulture, ".{0}.", areaName);

            return query.Where(x => x.Key.IndexOf(areaNameToFind, StringComparison.OrdinalIgnoreCase) != -1);
        }

        public static IEnumerable<KeyValuePair<string, Type>> WithoutAreaName(
            this IEnumerable<KeyValuePair<string, Type>> query)
        {
            return query.Where(x => x.Key.IndexOf(".areas.", StringComparison.OrdinalIgnoreCase) == -1);
        }

        public static IEnumerable<KeyValuePair<string, Type>> ByControllerName(
            this IEnumerable<KeyValuePair<string, Type>> query, string controllerName)
        {
            var controllerNameToFind = string.Format(CultureInfo.InvariantCulture, ".{0}{1}", controllerName,
                                                     DefaultHttpControllerSelector.ControllerSuffix);

            return query.Where(x => x.Key.EndsWith(controllerNameToFind, StringComparison.OrdinalIgnoreCase));
        }
    }
}