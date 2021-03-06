﻿namespace Fabric.Terminology.IntegrationTests.Fixtures
{
    using Fabric.Terminology.API.Services;
    using Fabric.Terminology.Domain.Persistence.Querying;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;
    using Fabric.Terminology.SqlServer.Persistence.Ordering;
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

        public IClientTermCustomizationService ClientTermCustomizationService { get; private set; }

        public ICodeSystemService CodeSystemService { get; private set; }

        public ICodeSystemCodeService CodeSystemCodeService { get; private set; }

        private void Initialize()
        {
            var cacheManagerFactory = new CachingManagerFactory(this.Cache);
            var pagingStrategyFactory = new PagingStrategyFactory();
            var clientTermCacheManager = new ClientTermCacheManager(cacheManagerFactory);
            var uow = new ClientTermValueUnitOfWorkManager(this.ClientTermContext.AsLazy(), this.Logger);
            var valueSetStatusChangePolicy = new DefaultValueSetUpdateValidationPolicy();
            var orderingStrategyFactory = new OrderingStrategyFactory();

            var valueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory,
                orderingStrategyFactory);

            var valueSetCodeCountRepository = new SqlValueSetCodeCountRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory);

            var valueSetBackingItemRepository = new SqlValueSetBackingItemRepository(
                this.SharedContext,
                this.Logger,
                cacheManagerFactory,
                pagingStrategyFactory,
                orderingStrategyFactory);

            var sqlClientTermUowRepository = new SqlClientTermValueSetRepository(
                this.Logger,
                uow,
                valueSetStatusChangePolicy,
                clientTermCacheManager);

            var sqlCodeSystemRepository = new SqlCodeSystemRepository(
                this.SharedContext,
                this.Logger,
                new CodeSystemCachingManager(this.Cache));

            var sqlCodeSystemCodeRepository = new SqlCodeSystemCodeRepository(
                this.SharedContext,
                this.Logger,
                new CodeSystemCodeCachingManager(this.Cache),
                sqlCodeSystemRepository,
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

            this.ClientTermCustomizationService = new ClientTermCustomizationService(
                this.CodeSystemCodeService,
                this.ValueSetCodeService,
                this.ClientTermValueSetService);
        }
    }
}