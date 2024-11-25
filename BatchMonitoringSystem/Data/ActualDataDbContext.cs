using BatchMonitoringSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BatchMonitoringSystem.Data
{
    public class ActualDataDbContext : DbContext
    {
        public ActualDataDbContext(DbContextOptions<ActualDataDbContext> options)
            : base(options)
        {
        }

        public DbSet<tblActualData> tblActualData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                throw new InvalidOperationException("DbContext is not configured with a connection string.");
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configure entity relationships and constraints if needed
        }
    }

}
