using System.Text.RegularExpressions;

internal static class Helpers
{
    public static string RemoveHex(this string str)
    {
        return Regex.Replace(str, @"\[([\W\w]{6})\]", "");
    }
}