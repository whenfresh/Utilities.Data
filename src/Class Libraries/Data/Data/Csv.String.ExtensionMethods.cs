namespace WhenFresh.Utilities.Data.Data
{
    public static class CsvStringExtensionMethods
    {
        public static string FormatCommaSeparatedValue(this string value)
        {
            if (null == value)
            {
                return null;
            }

            if (0 == value.Length)
            {
                return string.Empty;
            }

            value = value.Replace("\"", "\"\"", StringComparison.Ordinal);

            return ' ' == value[0]
                   || ' ' == value[value.Length - 1]
                   || value.ContainsAny(',', '\n', '"')
                   || value.Contains(Environment.NewLine)
                       ? string.Concat("\"", value, "\"")
                       : value;
        }
    }
}