namespace Fabric.Terminology.SqlServer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Extension methods for <see cref="StringBuilder"/>
    /// </summary>
    public static partial class Extensions
    {
        internal static StringBuilder AppendEscaptedSqlString(this StringBuilder builder, string input)
        {
            builder.Append(EscapeForSqlString(input));
            return builder;
        }

        internal static StringBuilder AppendInClause(this StringBuilder builder, IEnumerable<string> values)
        {
            var escapedValues = values.Select(EscapeForSqlString).Select(v => "'" + v + "'");
            var csv = string.Join(",", escapedValues);
            builder.AppendFormat(" IN ({0})", csv);
            return builder;
        }

        internal static StringBuilder AppendInClause(this StringBuilder builder, IEnumerable<int> values)
        {
            var csv = string.Join(",", values);
            builder.AppendFormat(" IN ({0})", csv);
            return builder;
        }

        private static string EscapeForSqlString(string input)
        {
            return input.Replace("'", "''");
        }
    }
}
