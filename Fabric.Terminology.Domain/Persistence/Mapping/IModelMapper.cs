namespace Fabric.Terminology.Domain.Persistence.Mapping
{
    using JetBrains.Annotations;

    public interface IModelMapper<in TSrc, out TResult>
        where TSrc : class
    {
        [CanBeNull]
        TResult Map(TSrc dto);
    }
}