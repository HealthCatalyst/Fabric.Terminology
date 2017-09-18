namespace Fabric.Terminology.Domain.Strategy
{
    using System;

    using Fabric.Terminology.Domain.Models;

    [Obsolete("Use ClientTermFLG property")]
    public interface IIsCustomValueStrategy
    {
        bool Get(IValueSet valueSet);

        void Set(IValueSet valueSet);
    }
}