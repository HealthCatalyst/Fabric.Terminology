namespace Fabric.Terminology.SqlServer
{
    using System;
    using System.Collections.Generic;

    using JetBrains.Annotations;

    public static partial class SqlServerExtensions
    {
        /// <summary>
        /// An overload for Contains which allows a <see cref="StringComparison"/> type
        /// to be provided, for things like case-insensitive searches.
        /// </summary>
        /// <param name="source">The string to search</param>
        /// <param name="value">The value to search for in the string.</param>
        /// <param name="comparisonType">The string comparison type to use.</param>
        /// <returns>
        /// True if the <paramref name="value"/> is found in the given <paramref name="source"/>
        /// according to the given string comparison type.
        /// False otherwise.
        /// </returns>
        internal static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        /// <summary>
        /// Splits the given string by any whitespace separators.
        /// </summary>
        /// <param name="searchText">The string to tokenize.</param>
        /// <returns>A collection of the individual words found in the given <paramref name="searchText"/>.</returns>
        internal static IReadOnlyCollection<string> AsSplitSearchTokens([CanBeNull] this string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return Array.Empty<string>();
            }

            var emptyArrayThatRepresentsWhitespaceForStringSplit = Array.Empty<char>();
            return searchText.Split(
                emptyArrayThatRepresentsWhitespaceForStringSplit,
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
