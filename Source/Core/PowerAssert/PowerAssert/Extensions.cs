namespace PowerAssertForked
{
    using System.Linq;
    using System.Text.RegularExpressions;

    internal static class Extensions
    {
        public static string CleanupName(this string name)
        {
            var tmp = name.CleanupUnderScores();
            return tmp.CleanupCamelCasing();
        }

        public static string CleanupUnderScores(this string name)
        {
            if (name.Contains('_'))
                return name.Replace('_', ' ');
            return name;
        }

        public static string CleanupCamelCasing(this string name)
        {
            return Regex.Replace(name,
            "([A-Z])",
            " $1",
            RegexOptions.Compiled
            ).Trim();
        }
    }
}
