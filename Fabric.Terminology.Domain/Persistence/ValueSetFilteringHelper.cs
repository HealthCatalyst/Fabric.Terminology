namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    public static class ValueSetFilteringHelper
    {
        public static IPagerSettings ValidateValueSetOrdering(IPagerSettings settings)
        {
            var vso = GetValidValueSetOrdering(settings.OrderBy, settings.Direction.ToString());
            settings.Direction = vso.Direction;
            settings.OrderBy = vso.FieldName;
            return settings;
        }

        public static ValueSetOrderingParameters GetValidValueSetOrdering(string field, string direction)
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