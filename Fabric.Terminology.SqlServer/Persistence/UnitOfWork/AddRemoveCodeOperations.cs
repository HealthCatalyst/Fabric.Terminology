namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System.Collections.Generic;

    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal class AddRemoveCodeOperations
    {
        public IReadOnlyCollection<ValueSetCodeDto> CurrentCodeDtos { get; set; }

        public IReadOnlyCollection<Operation> Operations { get; set; }
    }
}
