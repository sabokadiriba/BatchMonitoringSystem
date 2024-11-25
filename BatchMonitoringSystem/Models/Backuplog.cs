public class BackupLog
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string FolderName { get; set; }
    public int UserId { get; set; }
    public string LocalDestinationPath { get; set; }
    public string RemoteDestinationPath { get; set; }
    public int NumberOfUniqueBatches { get; set; }
    public int NumberOfDuplicateBatches { get; set; }
    public int TotalNumberOfBatches { get; set; }
    public string BackupUser { get; set; }
    public DateTime BackupDate { get; set; }
}
