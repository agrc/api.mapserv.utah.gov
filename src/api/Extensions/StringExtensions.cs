namespace AGRC.api.Extensions;
public static class StringExtensions {
    public static string ToCamelCase(this string value)
        => char.ToLowerInvariant(value[0]) + value[1..];
}
