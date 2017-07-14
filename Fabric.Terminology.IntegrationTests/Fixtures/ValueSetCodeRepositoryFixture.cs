﻿namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.SqlServer.Models.Dto;
    using Fabric.Terminology.SqlServer.Persistence;

    public class ValueSetCodeRepositoryFixture : RepositoryFixtureBase
    {
        public ValueSetCodeRepositoryFixture()
        {
            this.Initialize();
        }

        public IValueSetCodeRepository ValueSetCodeRepository { get; private set; }

        private void Initialize()
        {
            this.ValueSetCodeRepository = new SqlValueSetCodeRepository(this.SharedContext, this.Logger, new DefaultPagingStrategy<ValueSetCodeDto, IValueSetCode>(20));
        }
    }
}