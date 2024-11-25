using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BatchMonitoringSystem.Services
{
    public interface IActualDataDbContextFactory
    {
        ActualDataDbContext CreateDbContext(Equipment equipment);
    }

    public class ActualDataDbContextFactory : IActualDataDbContextFactory
    {
        public ActualDataDbContext CreateDbContext(Equipment equipment)
        {
            if (equipment == null)
                throw new ArgumentNullException(nameof(equipment));
            if (string.IsNullOrEmpty(equipment.IP) || string.IsNullOrEmpty(equipment.EquipmentName) ||
                string.IsNullOrEmpty(equipment.SqlUserName) || string.IsNullOrEmpty(equipment.SqlPassword))
            {
                throw new ArgumentException("Invalid equipment details provided.");
            }
            string connectionString = $"Server=(localdb)\\mssqllocaldb;Database=BatchMonitoringSystem;TrustServerCertificate=True;";

            var optionsBuilder = new DbContextOptionsBuilder<ActualDataDbContext>()
                .UseSqlServer(connectionString);
            return new ActualDataDbContext(optionsBuilder.Options);
        }
    }



}
