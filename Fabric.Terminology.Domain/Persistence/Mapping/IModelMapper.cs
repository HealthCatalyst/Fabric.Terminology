namespace Fabric.Terminology.Domain.Persistence.Mapping
{
    using JetBrains.Annotations;

    public interface IModelMapper<TDto, TResult>
        where TDto : class
    {
        [CanBeNull]
        TResult Map(TDto dto);

        //TDto Map(TResult result);
    }
}