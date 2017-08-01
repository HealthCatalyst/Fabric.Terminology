namespace Fabric.Terminology.Domain.Persistence.Mapping
{
    using JetBrains.Annotations;

    public interface IModelMapper<in TDto, out TResult>
        where TDto : class, new()
    {
        [CanBeNull]
        TResult Map(TDto dto);
    }
}