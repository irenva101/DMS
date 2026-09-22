using Microsoft.EntityFrameworkCore;

namespace DMS.Library.Data
{
    public class DmsContext : DbContext
    {
        public DmsContext(DbContextOptions<DmsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dms");
            modelBuilder.UseSerialColumns();


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DmsContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
        //public DbSet<ImportFileEntity> ImportFiles { get; set; }
    }
}
