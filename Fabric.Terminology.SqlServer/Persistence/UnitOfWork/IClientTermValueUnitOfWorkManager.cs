namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System;
    using System.Collections.Generic;

    using CallMeMaybe;

    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence.DataContext;

    internal interface IClientTermValueUnitOfWorkManager : IBulkCopyUnitOfWorkManager
    {
        ClientTermContext Context { get; }

        Maybe<ValueSetDescriptionBaseDto> GetValueSetDescriptionDto(Guid valueSetGuid);

        IReadOnlyCollection<ValueSetCodeDto> GetCodeDtos(Guid valueSetGuid);

        IReadOnlyCollection<ValueSetCodeCountDto> GetCodeCountDtos(Guid valueSetGuid);
    }
}