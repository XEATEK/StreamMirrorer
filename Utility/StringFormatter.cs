using System.Text.RegularExpressions;

namespace StreamMirrorer.Utility;

public static partial class StringFormatter
{
    [GeneratedRegex(@"\{(.+?)\}")]
    private static partial Regex NamedPlaceholdersRegex();
    
    public static string ReplaceNamedPlaceholders(string template, IDictionary<string, object> values)
    {
        return NamedPlaceholdersRegex().Replace(template, match =>
        {
            string key = match.Groups[1].Value;
            return (values.TryGetValue(key, out object? value) ? value.ToString() : match.Value) ?? throw new ArgumentNullException(paramName: key, message: "Null reference for string placeholder");
        });
    }
}