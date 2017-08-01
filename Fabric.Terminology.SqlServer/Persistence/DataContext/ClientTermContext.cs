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

        public DbSet<ValueSetDescriptionDto> ValueSetDescriptions { get; set; }

        // Used for testing
        internal bool IsInMemory { get; set; }

        internal TerminologySqlSettings Settings { get; }

        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            modelBuilder.Entity<ValueSetCodeDto>().ToTable("ValueSetCodeBASE", "ClientTerm");
            modelBuilder.Entity<ValueSetCodeDto>().Property(e => e.BindingNM).IsUnicode(false);
            modelBuilder.Entity<ValueSetCodeDto>()
                .HasKey(
                    code => new
                    {
                        code.BindingID,
                        code.BindingNM,
                        code.CodeCD,
                        code.LastLoadDTS,
                        code.SourceDSC,
                        code.ValueSetID,
                        code.VersionDSC
                    });

            modelBuilder.Entity<ValueSetDescriptionDto>().ToTable("ValueSetDescriptionBASE", "ClientTerm");
            modelBuilder.Entity<ValueSetDescriptionDto>().Property(e => e.BindingNM).IsUnicode(false);
            modelBuilder.Entity<ValueSetDescriptionDto>()
                .HasKey(e => e.ValueSetUniqueID);
            base.OnModelCreating(modelBuilder);
        }
    }
}
