using System;
using System.Collections.Generic;
using System.Text;

namespace Fabric.Terminology.Domain
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;

    public static partial class Extensions
    {
        public static void SetIdsForCustomInsert(this IValueSet valueSet)
        {
            var sequentialGuid = GuidComb.GenerateComb().ToString();

            ((ValueSet)valueSet).ValueSetUniqueId = sequentialGuid;
            ((ValueSet)valueSet).ValueSetId = sequentialGuid;
            ((ValueSet)valueSet).ValueSetOId = sequentialGuid;

            foreach (var code in valueSet.ValueSetCodes)
            {
                ((ValueSetCode)code).ValueSetUniqueId = sequentialGuid;
                ((ValueSetCode)code).ValueSetId = sequentialGuid;
                ((ValueSetCode)code).ValueSetOId = sequentialGuid;
                ((ValueSetCode)code).ValueSetName = valueSet.Name;
            }
        }

    }
}
