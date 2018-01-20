namespace Fabric.Terminology.Domain.Services
{
    using Fabric.Terminology.Domain.Models;

    public interface IValueSetUpdateValidationPolicy
    {
        bool CanBeDeleted(IValueSet valueSet);

        bool CanChangeStatus(ValueSetStatus current, ValueSetStatus target);
    }
}