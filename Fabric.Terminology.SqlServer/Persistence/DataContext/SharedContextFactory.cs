using System;
using Fabric.Terminology.SqlServer.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;


namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    using JetBrains.Annotations;

    internal class SharedContextFactory
    {
        private readonly TerminologySqlSettings settings;
        private readonly ILogger logger;
        [CanBeNull]
        private readonly ISeededDatabaseInitializer<SharedContext> seededDatabaseInitializer;

        public SharedContextFactory(TerminologySqlSettings sqlSettings, ILogger logger)
            : this(sqlSettings, logger, null)
        {            
        }

        internal SharedContextFactory(TerminologySqlSettings settings, ILogger logger, ISeededDatabaseInitializer<SharedContext> seededDatabaseInitializer = null)
        {
            this.settings = settings;
            this.logger = logger;
            this.seededDatabaseInitializer = seededDatabaseInitializer;
        }

        public SharedContext Create()
        {
            var context = this.settings.UseInMemory ? this.CreateInMemory() : this.CreateAttached();
            return context;
        }

        private SharedContext CreateAttached()
        {
            var builder = new DbContextOptionsBuilder<SharedContext>();
            builder.UseSqlServer(this.settings.ConnectionString);
            if (this.settings.LogGeneratedSql)
            {
                var lf = new LoggerFactory();
                lf.AddSerilog(this.logger);
                builder.UseLoggerFactory(lf);

            }
            return new SharedContext(builder.Options, this.settings) { IsInMemory = false };
        }

        private SharedContext CreateInMemory()
        {
            var builder = new DbContextOptionsBuilder<SharedContext>();
            builder.UseInMemoryDatabase();
            var context = new SharedContext(builder.Options, this.settings) { IsInMemory =  true };
            this.seededDatabaseInitializer?.Initialize(context);
            return context;
        }        
    }
}