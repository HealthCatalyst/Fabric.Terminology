using System;
using Fabric.Terminology.SqlServer.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    internal class SharedContextFactory
    {
        private readonly TerminologySqlSettings _settings;

        public SharedContextFactory(TerminologySqlSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public SharedContext Create()
        {
            return _settings.UseInMemory ? CreateInMemory() : CreateAttached();
        }

        private SharedContext CreateAttached()
        {
            var builder = new DbContextOptionsBuilder<SharedContext>();
            builder.UseSqlServer(_settings.ConnectionString);
            return new SharedContext(builder.Options);
        }

        private SharedContext CreateInMemory()
        {
            var builder = new DbContextOptionsBuilder<SharedContext>();
            builder.UseInMemoryDatabase();
            return new SharedContext(builder.Options);
        }        
    }
}