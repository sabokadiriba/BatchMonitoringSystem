namespace BatchMonitoringSystem.Models
{
    public class ViewUserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Department { get; set; }
      
        public List<string> Roles { get; set; } // Store roles as a list of strings
     
    }
}
