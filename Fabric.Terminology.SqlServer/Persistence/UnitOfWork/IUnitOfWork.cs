﻿namespace Fabric.Terminology.SqlServer.Persistence.UnitOfWork
{
    using System.Collections.Generic;


    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;

    internal interface IUnitOfWork
    {
        void Commit(Queue<Operation> operations);

        void Commit(Queue<Operation> operations, IReadOnlyCollection<ValueSetCodeDto> codes);
    }
}