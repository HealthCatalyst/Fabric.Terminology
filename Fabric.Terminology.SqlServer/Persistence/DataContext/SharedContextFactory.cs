using System;
using Fabric.Terminology.SqlServer.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;


namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    internal class SharedContextFactory
    {
        private readonly TerminologySqlSettings _settings;
        private readonly ILogger _logger;
        private readonly ISeededDatabaseInitializer<SharedContext> _seededDatabaseInitializer;

        public SharedContextFactory(TerminologySqlSettings settings, ILogger logger)
            : this(settings, logger, null)
        {            
        }

        internal SharedContextFactory(TerminologySqlSettings settings, ILogger logger, ISeededDatabaseInitializer<SharedContext> seededDatabaseInitializer = null)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentException(nameof(settings));
            _seededDatabaseInitializer = seededDatabaseInitializer;
        }

        public SharedContext Create()
        {
            var context = _settings.UseInMemory ? CreateInMemory() : CreateAttached();
            return context;
        }

        private SharedContext CreateAttached()
        {
            var builder = new DbContextOptionsBuilder<SharedContext>();
            builder.UseSqlServer(_settings.ConnectionString);
            if (_settings.LogGeneratedSql)
            {
                var lf = new LoggerFactory();
                lf.AddSerilog(_logger);
                builder.UseLoggerFactory(lf);

            }
            return new SharedContext(builder.Options, _settings) { IsInMemory = false };
        }

        private SharedContext CreateInMemory()
        {
            var builder = new DbContextOptionsBuilder<SharedContext>();
            builder.UseInMemoryDatabase();
            var context = new SharedContext(builder.Options, _settings) { IsInMemory =  true };
            _seededDatabaseInitializer?.Initialize(context);
            return context;
        }        
    }
}