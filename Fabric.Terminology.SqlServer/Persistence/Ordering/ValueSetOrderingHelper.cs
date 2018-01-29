namespace Fabric.Terminology.SqlServer.Persistence.Ordering
{
    using System;

    using Fabric.Terminology.Domain;
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence.Querying;

    public static class ValueSetOrderingHelper
    {
        public static IPagerSettings ValidateValueSetOrdering(IPagerSettings settings)
        {
            var vso = GetValidValueSetOrdering(settings.OrderBy, settings.Direction.ToString());
            settings.Direction = vso.Direction;
            settings.OrderBy = vso.FieldName;
            return settings;
        }

        public static IOrderingParameters GetValidValueSetOrdering(string field, string direction)
        {
            if (!Enum.TryParse(direction, true, out SortDirection sortDirection))
            {
                sortDirection = SortDirection.Asc;
            }

            var vso = new ValueSetOrderingParameters();
            if (field.IsNullOrWhiteSpace())
            {
                return vso;
            }

            vso.FieldName = field;
            vso.Direction = sortDirection;
            return vso;
        }
    }
}