namespace BatchMonitoringSystem.ViewModels
{
    public class UserActivityReportModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<UserActivityDto> Activities { get; set; } = new List<UserActivityDto>();
    }

    public class UserActivityDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
