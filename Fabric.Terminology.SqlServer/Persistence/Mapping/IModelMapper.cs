namespace Fabric.Terminology.SqlServer.Persistence.Mapping
{
    using System.Threading.Tasks;

    using Fabric.Terminology.Domain.Models;

    internal interface IModelMapper<in TDto, out TModel>
        where TDto : class
    {
        TModel Map(TDto dto);
    }
}