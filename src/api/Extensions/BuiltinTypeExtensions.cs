namespace AGRC.api.Extensions;
public static class BuiltinTypeExtensions {
    public static string ToCamelCase(this string source) {
        if (string.IsNullOrWhiteSpace(source)) {
            return string.Empty;
        }

        var parts = source.SplitOnToParts();
        var result = parts[0];

        for (var i = 1; i < parts.Count; i++) {
            var name = parts[i];
            result += $"{char.ToUpperInvariant(name[0])}{name[1..]}";
        }

        return result;
    }

    public static IList<string> SplitOnToParts(this string source) {
        var parts = new List<string>();
        var lexeme = new List<string>();

        for (var i = 0; i < source.Length; i++) {
            if (source[i].IsDelimiter() || source[i].IsUpperCase()) {
                if (lexeme.Count > 0) {
                    parts.Add(string.Concat(lexeme));
                    lexeme.Clear();
                }

                if (!source[i].IsDelimiter()) {
                    lexeme.Add(source[i].ToString().ToLowerInvariant());
                }
            } else {
                lexeme.Add(source[i].ToString().ToLowerInvariant());
            }
        }

        if (lexeme.Count > 0)
            parts.Add(string.Concat(lexeme));

        return parts;
    }

    public static bool IsUpperCase(this char symbol)
    => symbol >= 'A' && symbol <= 'Z';

    /// <summary>
    /// Tests a symbol for being a delimiter
    /// </summary>
    /// <param name="symbol">tested symbol</param>
    /// <returns></returns>
    public static bool IsDelimiter(this char symbol)
    => symbol == ' ' || symbol == '_' || symbol == '.' || symbol == '-';
}
