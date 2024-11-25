using BatchMonitoringSystem.Controllers;
using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BatchMonitoringSystem.Services
{
    public class UserActivityService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserActivityService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task LogActivityAsync(int userId, string activityType, string activityDescription)
        {
            var log = new UserActivityLog
            {
                UserId = userId,
                ActivityType = activityType,
                ActivityDescription = activityDescription,
                Timestamp = DateTime.Now
            };

            _context.UserActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }


        public async Task<UserActivityReportModel> GetUserActivitiesAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.UserActivityLogs.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= endDate.Value);
            }

            var activities = await query
                .Select(a => new UserActivityDto
                {
                    Timestamp = a.Timestamp,                   
                    UserName = _context.Users.FirstOrDefault(u => u.Id == a.UserId).UserName,
                    ActivityType = a.ActivityType,
                    ActivityDescription = a.ActivityDescription
                })
                .ToListAsync();

            return new UserActivityReportModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Activities = activities
            };
        }
    }

}
