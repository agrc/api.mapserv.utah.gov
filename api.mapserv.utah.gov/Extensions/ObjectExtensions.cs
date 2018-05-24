using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace api.mapserv.utah.gov.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object request, string separator = ",")
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                                    .Where(x =>
                                    {
                                        if (!x.CanRead)
                                        {
                                            return false;
                                        }

                                        var displayVariable = !TryGetAttribute(x, out JsonIgnoreAttribute prop);

                                        return displayVariable;
                                    })
                                    //.Where(x => x.GetValue(request, null) != null)
                                    .ToDictionary(x =>
                                    {
                                        var name = x.Name;
                                        if (TryGetAttribute(x, out JsonPropertyAttribute prop))
                                        {
                                            name = prop.PropertyName;
                                        }
                                        return name;
                                    }, x =>
                                    {
                                        var v = x.GetValue(request, null);
                                        return v ?? string.Empty;
                                    });

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                    ? valueType.GetGenericArguments()[0]
                    : valueType.GetElementType();

                if (!valueElemType.IsPrimitive && valueElemType != typeof(string))
                {
                    continue;
                }

                if (properties[key] is IEnumerable enumerable)
                {
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties.Select(x => string.Concat(Uri.EscapeDataString(x.Key), "=",
                                                                         Uri.EscapeDataString(x.Value.ToString()))));
        }

        public static bool TryGetAttribute<T>(PropertyInfo memberInfo, out T customAttribute) where T : Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            if (attributes == null)
            {
                customAttribute = null;
                return false;
            }
            customAttribute = (T) attributes;
            return true;
        }
    }
}
