namespace BatchMonitoringSystem.ViewModels
{
    public class BackupReportModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<BackupLog> BackupLogs { get; set; }
    }
}
