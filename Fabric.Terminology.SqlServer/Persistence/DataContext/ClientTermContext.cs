namespace Fabric.Terminology.SqlServer.Persistence.DataContext
{
    using System;

    using Fabric.Terminology.SqlServer.Configuration;
    using Fabric.Terminology.SqlServer.Models.Dto;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    internal class ClientTermContext : DbContext
    {
        public ClientTermContext(DbContextOptions options, TerminologySqlSettings settings)
            : base(options)
        {
            this.Settings = settings;
        }

        public DbSet<ValueSetCodeDto> ValueSetCodes { get; set; }

        public DbSet<ValueSetDescriptionBaseDto> ValueSetDescriptions { get; set; }

        public DbSet<ValueSetCodeCountDto> ValueSetCodeCounts { get; set; }

        // Used for testing
        internal bool IsInMemory { get; set; }

        internal TerminologySqlSettings Settings { get; }

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<ValueSetCodeDto>().ToTable("ValueSetCodeBASE", "ClientTerm");
            modelBuilder.Entity<ValueSetCodeDto>().HasKey(code => new { code.CodeCD, code.CodeSystemGuid, code.ValueSetGUID });

            modelBuilder.Entity<ValueSetCodeCountDto>().ToTable("ValueSetCodeCountBASE", "ClientTerm");
            modelBuilder.Entity<ValueSetCodeCountDto>().HasKey(record => new { record.ValueSetGUID, record.CodeSystemGUID });

            modelBuilder.Entity<ValueSetDescriptionBaseDto>().ToTable("ValueSetDescriptionBASE", "ClientTerm");
            modelBuilder.Entity<ValueSetDescriptionBaseDto>().HasKey(e => e.ValueSetGUID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
