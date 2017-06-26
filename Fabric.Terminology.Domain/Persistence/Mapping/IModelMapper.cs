namespace Fabric.Terminology.Domain.Persistence.Mapping
{
    public interface IModelMapper<in TSrc, out TResult>
        where TSrc : class
    {
        TResult Map(TSrc dto);
    }
}