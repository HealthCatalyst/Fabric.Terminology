namespace Fabric.Terminology.Domain.Models
{
    using System;

    /// <summary>
    /// Marker interface for a ValueSetCode
    /// </summary>
    public interface IValueSetCode : ICodeSystemCode, IHaveValueSetGuid
    {
    }
}