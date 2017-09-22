namespace Fabric.Terminology.TestsBase.Fixtures
{
    using Fabric.Terminology.Domain.Models;
    using Fabric.Terminology.Domain.Services;
    using Fabric.Terminology.SqlServer.Caching;
    using Fabric.Terminology.SqlServer.Persistence;

    public class CustomValueSetFixture : RepositoryFixtureBase
    {
        public CustomValueSetFixture()
        {
            this.Initialize();
        }

        internal SqlValueSetCodeRepository ValueSetCodeRepository { get; private set; }


        //internal IValueSetService ValueSetService { get; private set; }

        protected override bool ClientTermContextInMemory => true;

        private void Initialize()
        {
            this.ValueSetCodeRepository = new SqlValueSetCodeRepository(
                this.SharedContext,
                this.Logger,
                new ValueSetCachingManager<IValueSetCode>(this.Cache));

            //this.ValueSetService = new ValueSetService(this.ValueSetRepository);
        }
    }
}