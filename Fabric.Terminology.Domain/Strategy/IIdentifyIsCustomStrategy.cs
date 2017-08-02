namespace Fabric.Terminology.Domain.Strategy
{
    using Fabric.Terminology.Domain.Models;

    public interface IIdentifyIsCustomStrategy
    {
        bool Execute(IValueSet valueSet);

        void SetIsCustom(IValueSet valueSet);
    }
}