namespace Fabric.Terminology.Domain
{
    using Fabric.Terminology.Domain.Models;

    /// <summary>
    /// Extension methods for <see cref="ICodeSetCode"/>
    /// </summary>
    public static partial class Extensions
    {
        public static IValueSetCode AsCodeForValueSet(this ICodeSetCode codeSetCode, IValueSet valueSet)
        {
            return new ValueSetCode(
                valueSet.ValueSetId,
                valueSet.ValueSetUniqueId,
                valueSet.ValueSetOId,
                valueSet.Name,
                codeSetCode.Code,
                codeSetCode.CodeSystem,
                codeSetCode.VersionDescription,
                codeSetCode.SourceDescription,
                codeSetCode.LastLoadDate,
                codeSetCode.RevisionDate);
        }

        public static IValueSetCode AsCodeForValueSet(this ICodeSetCode codeSetCode, string valueSetId, string valueSetName)
        {
            return new ValueSetCode(
                valueSetId,
                valueSetId,
                valueSetId,
                valueSetName,
                codeSetCode.Code,
                codeSetCode.CodeSystem,
                codeSetCode.VersionDescription,
                codeSetCode.SourceDescription,
                codeSetCode.LastLoadDate,
                codeSetCode.RevisionDate);
        }
    }
}