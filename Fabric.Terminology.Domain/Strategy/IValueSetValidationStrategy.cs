namespace Fabric.Terminology.Domain.Strategy
{
    using Fabric.Terminology.Domain.Models;

    public interface IValueSetValidationStrategy
    {
        bool EnsureIsCustom(IValueSet valueSet);
    }
}