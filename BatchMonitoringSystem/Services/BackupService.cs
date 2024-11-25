using BatchMonitoringSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BatchMonitoringSystem.Services
{
    public class BackupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public BackupService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<bool> BackupAsync(int equipmentId, string sourcePath, string localDestinationPath, string remoteDestinationPath)
        {
            try
            {
                // Validate source and destination paths
                ValidateDirectory(sourcePath);

                // Ensure destination directories exist
                EnsureDirectoryExists(localDestinationPath);
                EnsureDirectoryExists(remoteDestinationPath);

                // Copy files from source to local destination
                CopyFiles(sourcePath, localDestinationPath);

                // Copy files from local to remote destination
                CopyFiles(localDestinationPath, remoteDestinationPath);

                // Record all folder names in the database
                var userId = _userService.GetCurrentUserId();
                var backupLog = await CreateBackupLog(equipmentId, sourcePath, localDestinationPath, remoteDestinationPath, userId);

                // Save backup log to the database
                _context.BackupLogs.Add(backupLog);
                await _context.SaveChangesAsync();

                // Delete subdirectories from source path after the backup process is complete
                DeleteSubdirectories(sourcePath);

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (or handle it as necessary)
                Console.WriteLine($"An error occurred during backup: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<BackupLog>> GetBackupLogsAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.BackupLogs.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(log => log.BackupDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(log => log.BackupDate <= endDate.Value);

            return await query.ToListAsync();
        }
        private void ValidateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory '{path}' does not exist.");
            }
        }

        private void EnsureDirectoryExists(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }

        private void CopyFiles(string sourcePath, string destinationPath)
        {
            var sourceDirectory = new DirectoryInfo(sourcePath);
            var destinationDirectory = new DirectoryInfo(destinationPath);

            // Create destination directory if it does not exist
            EnsureDirectoryExists(destinationPath);

            // Copy all files
            foreach (var file in sourceDirectory.GetFiles())
            {
                var destFile = Path.Combine(destinationDirectory.FullName, file.Name);
                file.CopyTo(destFile, true);
            }

            // Copy all subdirectories
            foreach (var subDir in sourceDirectory.GetDirectories())
            {
                var destSubDir = Path.Combine(destinationDirectory.FullName, subDir.Name);
                CopyFiles(subDir.FullName, destSubDir);
            }
        }

        private void DeleteSubdirectories(string path)
        {
            var directory = new DirectoryInfo(path);

            // Delete all files in the directory
            foreach (var file in directory.GetFiles())
            {
                file.Delete();
            }

            // Delete all subdirectories
            foreach (var subDir in directory.GetDirectories())
            {
                DeleteSubdirectories(subDir.FullName);
                subDir.Delete();
            }
        }

        private async Task<BackupLog> CreateBackupLog(int equipmentId, string sourcePath, string localDestinationPath, string remoteDestinationPath, int userId)
        {
            var backupLog = new BackupLog
            {
                EquipmentId = equipmentId,
                FolderName = GetFolderName(sourcePath),
                UserId = userId,
                LocalDestinationPath = localDestinationPath,
                RemoteDestinationPath = remoteDestinationPath,
                NumberOfUniqueBatches = CountUniqueBatches(sourcePath),
                NumberOfDuplicateBatches = CountDuplicateBatches(sourcePath),
                TotalNumberOfBatches = CountTotalBatches(sourcePath),
                BackupUser = await _userService.GetUsernameById(userId),
                BackupDate = DateTime.UtcNow
            };

            return backupLog;
        }

        private string GetFolderName(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return directoryInfo.Name;
        }

        private int CountUniqueBatches(string path)
        {
            var directories = Directory.GetDirectories(path);
            var uniqueDirectories = directories.Select(Path.GetFileName).Distinct();
            return uniqueDirectories.Count();
        }

        private int CountDuplicateBatches(string path)
        {
            // Use a list to collect all directories
            var allDirectories = new List<string>();

            // Traverse the directory structure and collect all directories
            TraverseDirectories(path, allDirectories);

            // Count the directories with names containing "_copy"
            return allDirectories.Count(d => Path.GetFileName(d).Contains("_copy") || Path.GetFileName(d).Contains("Copy"));
        }

        private void TraverseDirectories(string path, List<string> directories)
        {
            // Add the current directory to the list
            directories.Add(path);

            // Recursively traverse subdirectories
            foreach (var subDir in Directory.GetDirectories(path))
            {
                TraverseDirectories(subDir, directories);
            }
        }

        private int CountTotalBatches(string path)
        {
            // Logic to count total batches
            return Directory.GetDirectories(path).Length;
        }
    }
}
