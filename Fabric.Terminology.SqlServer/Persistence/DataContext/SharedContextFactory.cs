namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    using Fabric.Terminology.SqlServer.Configuration;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class SharedContextFactory : DbContextFactoryBase<SharedContext>
    {
        public SharedContextFactory(TerminologySqlSettings sqlSettings, ILogger logger)
            : this(sqlSettings, logger, null)
        {
        }

        internal SharedContextFactory(
            TerminologySqlSettings settings,
            ILogger logger,
            ISeededDatabaseInitializer<SharedContext> seededDatabaseInitializer = null)
            : base(settings, logger, seededDatabaseInitializer)
        {
        }

        protected override SharedContext GetInstance(
            DbContextOptionsBuilder<SharedContext> builder,
            bool isInMemory = false)
        {
            return new SharedContext(builder.Options, this.Settings) { IsInMemory = isInMemory };
        }
    }
}