using Fabric.Terminology.SqlServer.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Fabric.Terminology.SqlServer.Persistence
{
    internal class SharedContext : DbContext
    {        
        public SharedContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<ValueSetCodeDto> ValueSetCodes { get; set; }
        public DbSet<ValueSetDescriptionDto> ValueSetDescriptions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueSetCodeDto>().Property(e => e.BindingNM).IsUnicode(false);
            modelBuilder.Entity<ValueSetDescriptionDto>().Property(e => e.BindingNM).IsUnicode(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}