namespace Fabric.Terminology.SqlServer
{
    using System;

    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.SqlServer.Models;
    using Fabric.Terminology.SqlServer.Models.Dto;

    public static partial class Extensions
    {
        private static readonly EmptySamdBinding EmptyBinding = new EmptySamdBinding();

        internal static ValueSetDescriptionDto AsDto(this IValueSet valueSet)
        {
            throw new NotImplementedException();

            //return new ValueSetDescriptionDto
            //{
            //    AuthoringSourceDSC = valueSet.AuthoringSourceDescription,
            //    BindingID = EmptyBinding.BindingID,
            //    BindingNM = EmptyBinding.BindingNM,
            //    CreatedByNM = Constants.ValueSetDescriptionDto.CreatedByNM,
            //    DefinitionDSC = string.Empty,
            //    VersionDSC = valueSet.VersionDescription,
            //    LastLoadDTS = EmptyBinding.GetLastLoadDTS(),
            //    PublicFLG = "Y",
            //    LatestVersionFLG = "Y",
            //    StatusCD = "Active",
            //    PurposeDSC = valueSet.DefinitionDescription,
            //    RevisionDTS = null,
            //    SourceDSC = valueSet.SourceDescription,
            //    ValueSetUniqueID = valueSet.ValueSetUniqueId,
            //    ValueSetID = valueSet.ValueSetId,
            //    ValueSetOID = valueSet.ValueSetOId,
            //    ValueSetNM = valueSet.Name
            //};
        }

        internal static ValueSetCodeDto AsDto(this IValueSetCode code)
        {
            throw new NotImplementedException();

            //return new ValueSetCodeDto
            //{
            //    BindingNM = EmptyBinding.BindingNM,
            //    BindingID = EmptyBinding.BindingID,
            //    CodeCD = code.Code,
            //    CodeDSC = code.Name,
            //    CodeSystemNM = code.CodeSystem.Name,
            //    CodeSystemVersionTXT = code.CodeSystem.Version,
            //    CodeSystemCD = code.CodeSystem.Code,
            //    LastLoadDTS = code.LastLoadDate,
            //    SourceDSC = code.SourceDescription,
            //    ValueSetID = code.ValueSetId,
            //    ValueSetUniqueID = code.ValueSetUniqueId,
            //    ValueSetOID = code.ValueSetOId,
            //    ValueSetNM = code.ValueSetName,
            //    VersionDSC = code.VersionDescription
            //};
        }
    }
}
