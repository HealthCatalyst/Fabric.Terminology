namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    internal interface IModelMapper<in TDto, out TModel>
        where TDto : class
    {
        TModel Map(TDto dto);
    }
}