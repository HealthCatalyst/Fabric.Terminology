namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    using Fabric.Terminology.SqlServer.Configuration;

    using Microsoft.EntityFrameworkCore;

    using Serilog;

    internal class ClientTermContextFactory : DbContextFactoryBase<ClientTermContext>
    {
        public ClientTermContextFactory(TerminologySqlSettings sqlSettings, ILogger logger)
            : this(sqlSettings, logger, null)
        {
        }

        internal ClientTermContextFactory(
            TerminologySqlSettings settings,
            ILogger logger,
            ISeededDatabaseInitializer<ClientTermContext> seededDatabaseInitializer = null)
            : base(settings, logger, seededDatabaseInitializer)
        {
        }

        protected override ClientTermContext GetInstance(
            DbContextOptionsBuilder<ClientTermContext> builder,
            bool isInMemory = false)
        {
            return new ClientTermContext(builder.Options, this.Settings) { IsInMemory = isInMemory };
        }
    }
}