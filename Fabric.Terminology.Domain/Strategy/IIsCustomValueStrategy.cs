namespace Fabric.Terminology.Domain.Strategy
{
    using Fabric.Terminology.Domain.Models;

    public interface IIsCustomValueStrategy
    {
        bool Get(IValueSet valueSet);

        void Set(IValueSet valueSet);
    }
}