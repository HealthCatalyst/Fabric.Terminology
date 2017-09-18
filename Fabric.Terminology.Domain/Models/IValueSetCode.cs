namespace Fabric.Terminology.Domain.Models
{
    using System;

    public interface IValueSetCode : ICodeSetCode
    {
        Guid ValueSetGuid { get; }
    }
}