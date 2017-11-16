namespace Fabric.Terminology.Domain.Services
{
    public interface IValueSetStatusChangePolicy
    {
        bool Allowed(ValueSetStatus current, ValueSetStatus target);
    }
}