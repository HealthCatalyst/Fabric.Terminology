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

        public override SharedContext Create(bool useInMemory = false)
        {
            var context = base.Create(useInMemory);

            // Shared Terminology data is read only so there is no reason to ever track the entities.
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        protected override SharedContext GetInstance(
            DbContextOptionsBuilder<SharedContext> builder,
            bool isInMemory = false)
        {
            return new SharedContext(builder.Options, this.Settings) { IsInMemory = isInMemory };
        }
    }
}