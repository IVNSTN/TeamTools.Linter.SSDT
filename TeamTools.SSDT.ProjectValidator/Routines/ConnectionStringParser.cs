using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamTools.SSDT.ProjectValidator.Routines
{
    public static class ConnectionStringParser
    {
        private static readonly char[] OptionSeparator = new char[] { ';' };

        public static IDictionary<string, string> Parse(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return default;
            }

            var result = connectionString
                .Split(OptionSeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split('='))
                .Where(values => values.Length > 1)
                .ToDictionary(values => values[0].Trim(), values => values[1].Trim(), StringComparer.OrdinalIgnoreCase);

            return result;
        }
    }
}
