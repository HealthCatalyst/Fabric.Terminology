namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    using System;

    using Fabric.Terminology.SqlServer.Configuration;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.Logging;

    using Serilog;

    using ILogger = Serilog.ILogger;

    internal abstract class DbContextFactoryBase<TDbContext>
        where TDbContext : DbContext
    {
        protected DbContextFactoryBase(
            TerminologySqlSettings settings,
            ILogger logger,
            [CanBeNull]ISeededDatabaseInitializer<TDbContext> seededDatabaseInitializer)
        {
            this.Settings = settings;
            this.Logger = logger;
            this.SeededDatabaseInitializer = seededDatabaseInitializer;
        }

        protected TerminologySqlSettings Settings { get; }

        protected ILogger Logger { get; }

        [CanBeNull]
        protected ISeededDatabaseInitializer<TDbContext> SeededDatabaseInitializer { get; }

        public virtual TDbContext Create(bool useInMemory = false)
        {
            var context = useInMemory ? this.CreateInMemory() : this.CreateAttached();
            return context;
        }

        public Lazy<TDbContext> CreateLazy(bool useInMemory = false)
        {
            return new Lazy<TDbContext>(() => this.Create(useInMemory));
        }

        protected abstract TDbContext GetInstance(DbContextOptionsBuilder<TDbContext> builder, bool isInMemory = false);

        protected virtual TDbContext CreateAttached()
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();
            builder.UseSqlServer(this.Settings.ConnectionString);
            if (this.Settings.LogGeneratedSql)
            {
                var lf = new LoggerFactory();
                lf.AddSerilog(this.Logger);
                builder.UseLoggerFactory(lf);
            }

            var context = this.GetInstance(builder);
            this.SeededDatabaseInitializer?.Initialize(context);
            return context;
        }

        protected virtual TDbContext CreateInMemory()
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();
            builder.UseInMemoryDatabase();
            builder.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            var context = this.GetInstance(builder, true);
            this.SeededDatabaseInitializer?.Initialize(context);
            return context;
        }
    }
}