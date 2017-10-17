namespace Fabric.Terminology.SqlServer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    public static partial class Extensions
    {
        public static string GetTableForEntity<T>(this DbContext context)
        {
            return context.GetTableAndSchmeaForEntity(typeof(T));
        }

        public static string GetTableAndSchmeaForEntity(this DbContext context, Type type)
        {
            var mapping = context.Model.FindEntityType(type).Relational();
            return string.Join(
                ".",
                new[] { mapping.Schema, mapping.TableName }.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        /// <summary>
        /// Deletes via where statements rather than one entity at a time.
        /// </summary>
        /// <remarks>When DbExtensions supports .NET core, it might be worth refactoring this to use that package.</remarks>
        /// <returns> The number of rows affected. </returns>
        public static int BulkDelete(
            this DbContext context,
            IReadOnlyCollection<Type> entityTypes,
            IReadOnlyDictionary<string, object> columnCriteria,
            int batchSize = 500)
        {
            var totalCount = 0;

            if (!entityTypes.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(entityTypes));
            }

            if (!columnCriteria.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(columnCriteria));
            }

            if (batchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            object[] parameters = columnCriteria.Select(c => new SqlParameter($"@{c.Key}", c.Value)).ToArray();
            var whereConditions = string.Join(" AND ", columnCriteria.Select(c => $"{c.Key} = @{c.Key}"));
            foreach (var entityType in entityTypes)
            {
                var tableAndSchmeaForEntity = context.GetTableAndSchmeaForEntity(entityType);
                int executionCount;
                do
                {
                    var sql = $"DELETE TOP ({batchSize}) FROM {tableAndSchmeaForEntity} WHERE {whereConditions}";
                    executionCount = context.Database.ExecuteSqlCommand(sql, parameters);
                    totalCount += executionCount;
                }
                while (executionCount == batchSize);
            }

            return totalCount;
        }
    }
}
