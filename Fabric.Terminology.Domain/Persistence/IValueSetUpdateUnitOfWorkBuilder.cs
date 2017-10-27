namespace Fabric.Terminology.Domain.Persistence
{
    using System;
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Models;

    internal interface IValueSetUpdateUnitOfWorkBuilder
    {
        IUnitOfWork BuildAddCodesUnitOfWork(
            Guid valueSetGuid,
            IEnumerable<IValueSetCode> valueSetCodes);

        IUnitOfWork BuildRemoveCodesUnitOfWork(
            Guid valueSetGuid,
            IEnumerable<ICodeSystemCode> codeSystemCodes);
    }
}
