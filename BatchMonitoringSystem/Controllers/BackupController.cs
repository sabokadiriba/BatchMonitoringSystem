using Microsoft.AspNetCore.Mvc;
using BatchMonitoringSystem.Services;
namespace BatchMonitoringSystem.Controllers
{
    public class BackupController : Controller
    {
        private readonly BackupService _backupService;

        public BackupController(BackupService backupService)
        {
            _backupService = backupService;
        }

        [HttpGet]
        public IActionResult Backup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Backup(BackupRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _backupService.BackupAsync(
                    request.EquipmentId,
                    request.SourcePath,
                    request.LocalDestinationPath,
                    request.RemoteDestinationPath);

                if (result)
                {
                    return RedirectToAction("Success"); 
                }

                return StatusCode(500, "An error occurred during backup.");
            }

            return View(request);
        }

        public IActionResult Success()
        {
            return View();
        }
    }






    public class BackupRequest
    {
        public int EquipmentId { get; set; }
        public string SourcePath { get; set; }
        public string LocalDestinationPath { get; set; }
        public string RemoteDestinationPath { get; set; }
    }

}
