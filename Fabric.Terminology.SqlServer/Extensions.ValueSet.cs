namespace Fabric.Terminology.SqlServer
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.Mapping;

    public static partial class Extensions
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        internal static void ReadyForCustomInsert(this IValueSet valueSet)
        {
            var sequentialGuid = GuidComb.GenerateComb().ToString();

            valueSet.ValueSetUniqueId = sequentialGuid;
            valueSet.ValueSetId = sequentialGuid;
            valueSet.ValueSetOId = sequentialGuid;

            foreach (var code in valueSet.ValueSetCodes)
            {
                ((ValueSetCode)code).ValueSetUniqueId = sequentialGuid;
                ((ValueSetCode)code).ValueSetId = sequentialGuid;
                ((ValueSetCode)code).ValueSetOId = sequentialGuid;
                ((ValueSetCode)code).ValueSetName = valueSet.Name;
            }
        }

        internal static ValueSetDescriptionDto AsDto(this IValueSet valueSet)
        {
            return new ValueSetDescriptionDto
            {
                AuthoringSourceDSC = valueSet.AuthoringSourceDescription,
                BindingID = EmptyBinding.BindingID,
                BindingNM = EmptyBinding.BindingNM,
                CreatedByNM = Constants.ValueSetDescriptionDto.CreatedByNM,
                DefinitionDSC = string.Empty,
                LastLoadDTS = EmptyBinding.GetLastLoadDTS(),
                PublicFLG = "Y",
                LatestVersionFLG = "Y",
                PurposeDSC = valueSet.PurposeDescription,
                RevisionDTS = null,
                SourceDSC = valueSet.SourceDescription,
                ValueSetUniqueID = valueSet.ValueSetUniqueId,
                ValueSetID = valueSet.ValueSetId,
                ValueSetOID = valueSet.ValueSetOId,
                ValueSetNM = valueSet.Name
            };
        }

        internal static ValueSetCodeDto AsDto(this IValueSetCode code)
        {
            return new ValueSetCodeDto
            {
                BindingNM = EmptyBinding.BindingNM,
                BindingID = EmptyBinding.BindingID,
                CodeCD = code.Code,
                CodeDSC = code.Name,
                CodeSystemNM = code.CodeSystem.Name,
                CodeSystemVersionTXT = code.CodeSystem.Version,
                LastLoadDTS = code.LastLoadDate,
                SourceDSC = code.SourceDescription,
                ValueSetUniqueID = code.ValueSetUniqueId,
                ValueSetOID = code.ValueSetOId,
                ValueSetNM = code.ValueSetName,
                VersionDSC = code.VersionDescription
            };
        }
    }
}
