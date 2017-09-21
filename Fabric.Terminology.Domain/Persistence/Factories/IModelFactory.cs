namespace Fabric.Terminology.Domain.Persistence.Factories
{
    public interface IModelFactory<in TDto, out TResult>
        where TDto : class, new()
    {
        TResult Build(TDto dto);
    }
}