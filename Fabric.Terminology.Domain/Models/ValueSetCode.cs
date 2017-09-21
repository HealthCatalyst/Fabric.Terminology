namespace Fabric.Terminology.Domain.Models
{
    using System;

    internal class ValueSetCode : CodeSetCode, IValueSetCode
    {
        public Guid ValueSetGuid { get; internal set; }
    }
}