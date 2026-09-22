using DLMS.Tenants.Repositories.Models;
using DMS.Shared.Helper;
using Microsoft.EntityFrameworkCore;

namespace DLMS.Tenants.Data
{
    public class SharedContext : DbContext
    {
        public SharedContext(DbContextOptions<SharedContext> options)
            : base(options) { }

        public SharedContext() { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tenant");
            //permissions tables
            modelBuilder
                .Entity<PermissionEntity>()
                .HasMany(p => p.Roles)
                .WithMany(r => r.Permissions)
                .UsingEntity<Dictionary<string, object>>(
                    "permission_entity_role_entity", // Custom table name in lowercase
                    j =>
                        j.HasOne<RoleEntity>()
                            .WithMany()
                            .HasForeignKey("role_id")
                            .HasConstraintName("FK_PermissionRole_Role"),
                    j =>
                        j.HasOne<PermissionEntity>()
                            .WithMany()
                            .HasForeignKey("permission_id")
                            .HasConstraintName("FK_PermissionRole_Permission"),
                    j =>
                    {
                        j.Property<long>("role_id").HasColumnName("role_id");
                        j.Property<long>("permission_id").HasColumnName("permission_id");
                    }
                );

            modelBuilder.Entity<PermissionEntity>().HasIndex(p => p.Value).IsUnique();

            //user role relation 1:N
            modelBuilder
                .Entity<UserEntity>()
                .HasOne(r => r.Role)
                .WithMany(u => u.Users)
                .HasForeignKey(r => r.RoleId);

            // Register Enums in DB
            modelBuilder.RegisterEnums<SharedContext>();
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<UtilityEntity> Utilities { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<PermissionEntity> Permissions { get; set; }

    }
}
