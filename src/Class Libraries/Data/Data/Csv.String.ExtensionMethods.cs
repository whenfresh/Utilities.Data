namespace Cavity.Data
{
    using System;

    public static class CsvStringExtensionMethods
    {
#if NET20
        public static string FormatCommaSeparatedValue(string value)
#else
        public static string FormatCommaSeparatedValue(this string value)
#endif
        {
            if (null == value)
            {
                return null;
            }

            if (0 == value.Length)
            {
                return string.Empty;
            }

#if NET20
            value = StringExtensionMethods.Replace(value, "\"", "\"\"", StringComparison.Ordinal);
#else
            value = value.Replace("\"", "\"\"", StringComparison.Ordinal);
#endif

            return ' ' == value[0]
                   || ' ' == value[value.Length - 1]
#if NET20
                   || StringExtensionMethods.ContainsAny(value, ',', '\n', '"')
#else
                   || value.ContainsAny(',', '\n', '"')
#endif
                   || value.Contains(Environment.NewLine)
                       ? string.Concat("\"", value, "\"")
                       : value;
        }
    }
}