﻿namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.Domain.Persistence;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.UnitOfWork;
    using Fabric.Terminology.SqlServer.Services;
    using Fabric.Terminology.TestsBase.Fixtures;

    public class SqlServiceFixture : RepositoryFixtureBase
    {
        public SqlServiceFixture()
        {
            this.Initialize();
        }

        public IValueSetService ValueSetService { get; private set; }

        public IValueSetCodeService ValueSetCodeService { get; private set; }

        public IValueSetSummaryService ValueSetSummaryService { get; private set; }

        public IClientTermValueSetService ClientTermValueSetService { get; private set; }

        public ICodeSystemService CodeSystemService { get; private set; }

        public ICodeSystemCodeService CodeSystemCodeService { get; private set; }

        private void Initialize()
        {
            var cacheManagerFactory = new CachingManagerFactory(this.Cache);
            var pagingStrategyFactory = new PagingStrategyFactory();
            var uow = new ClientTermUnitOfWork(this.ClientTermContext.AsLazy(), this.Logger);

            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory);

            var valueSetCodeCountRepository = new SqlValueSetCodeCountRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory);

            var valueSetBackingItemRepository = new SqlValueSetBackingItemRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory);

            var sqlClientTermUowRepository = new SqlClientTermUnitOfWorkRepository(uow);

            var sqlCodeSystemRepository = new SqlCodeSystemRepository(
                this.SharedContext,
                this.Logger,
                new CodeSystemCachingManager(this.Cache));

            var sqlCodeSystemCodeRepository = new SqlCodeSystemCodeRepository(
                this.SharedContext,
                this.Logger,
                new CodeSystemCodeCachingManager(this.Cache),
                pagingStrategyFactory);

            this.ValueSetService = new SqlValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeRepository,
                valueSetCodeCountRepository);

            this.ValueSetCodeService = new SqlValueSetCodeService(valueSetCodeRepository);

            this.ClientTermValueSetService = new SqlClientTermValueSetService(
                this.Logger,
                valueSetBackingItemRepository,
                sqlClientTermUowRepository);

            this.ValueSetSummaryService = new SqlValueSetSummaryService(
                this.Logger,
                valueSetBackingItemRepository,
                valueSetCodeCountRepository);

            this.CodeSystemService = new SqlCodeSystemService(sqlCodeSystemRepository);

            this.CodeSystemCodeService = new SqlCodeSystemCodeService(sqlCodeSystemCodeRepository);
        }
    }
}