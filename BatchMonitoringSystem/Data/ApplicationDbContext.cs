using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BatchMonitoringSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductParameter> ProductParameters { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<tblActualData> tblActualData { get; set; }
        public DbSet<BatchParameter> BatchParameters { get; set; }
        public DbSet<BackupLog> BackupLogs { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure many-to-many relationship between ApplicationRole and Permission using RolePermission
            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
            builder.Entity<Product>()
            .HasMany(p => p.Parameters)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId);
        }
    }
}
