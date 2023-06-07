using System.Text.RegularExpressions;

public static class StringUtil {
    public static string FormatCamelCase(string input) {
        var formattedString = Regex.Replace(input, "([A-Z])", " $1").Trim();
        formattedString = char.ToUpper(formattedString[0]) + formattedString[1..];
        return formattedString;
    }
}