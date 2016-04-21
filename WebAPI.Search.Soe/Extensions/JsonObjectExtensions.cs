using System;
using ESRI.ArcGIS.SOESupport;

namespace WebAPI.Search.Soe.Extensions
{
    public static class JsonObjectExtensions
    {
        public static string GetStringValue(this JsonObject operationInput, string name, bool nullable = false)
        {
            string value;

            var found = operationInput.TryGetString(name, out value);
            if (!found || string.IsNullOrEmpty(value))
            {
                if (nullable)
                {
                    return "";
                }

                throw new ArgumentNullException(name);
            }

            return value;
        }

        public static double GetNumberValue(this JsonObject operationInput, string name, bool nullable = false)
        {
            double? value;

            var found = operationInput.TryGetAsDouble(name, out value);
            if (!found || !value.HasValue)
            {
                if (nullable)
                {
                    return -1;
                }

                throw new ArgumentNullException(name);
            }

            return value.Value;
        }
    }
}