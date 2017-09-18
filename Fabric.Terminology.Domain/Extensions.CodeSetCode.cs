namespace Fabric.Terminology.Domain
{
    using System;

    using Fabric.Terminology.Domain.Models;

    /// <summary>
    /// Extension methods for <see cref="ICodeSetCode"/>
    /// </summary>
    public static partial class Extensions
    {
        public static IValueSetCode AsCodeForValueSet(this ICodeSetCode codeSetCode, IValueSet valueSet)
        {
           throw new NotImplementedException();
        }

        public static IValueSetCode AsCodeForValueSet(this ICodeSetCode codeSetCode, string valueSetId, string valueSetName)
        {
            throw new NotImplementedException();
        }
    }
}