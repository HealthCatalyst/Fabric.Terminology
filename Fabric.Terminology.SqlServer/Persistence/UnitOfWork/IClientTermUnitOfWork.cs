using Fabric.Terminology.SqlServer.Models.Dto;

namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    using Serilog;

    internal interface IClientTermUnitOfWork : IUnitOfWork
    {
        ClientTermContext Context { get; }

        ILogger Logger { get; }

        IUnitOfWorkRepository<ValueSetCodeDto> ValueSetCodes { get; }

        IUnitOfWorkRepository<ValueSetCodeCountDto> ValueSetCodeCounts { get; }

        IUnitOfWorkRepository<ValueSetDescriptionBaseDto> ValueSetDescriptions { get; }
    }
}