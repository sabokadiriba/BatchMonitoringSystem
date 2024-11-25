
using BatchMonitoringSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using BatchMonitoringSystem.ViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace BatchMonitoringSystem.Controllers
{
   
    public class ReportsController : Controller
    {
        private readonly UserActivityService _activityService;
        private readonly ProductService _productService;
        private readonly BatchService _batchService;
        private readonly BackupService _backupService;
        private readonly IConverter _converter;
        public ReportsController(UserActivityService activityService,
            ProductService productService,
            BatchService batchService,
            BackupService backupService,
            IConverter converter)
        {
            _activityService = activityService; 
            _productService = productService;
            _batchService = batchService;
            _backupService = backupService;
            _converter = converter;
        }
        [Authorize(Policy = "ViewUserActivityReportPolicy")]
        public async Task<IActionResult> UserActivityReport(DateTime? startDate, DateTime? endDate)
        {
            var model = await _activityService.GetUserActivitiesAsync(startDate, endDate);
            return View(model);
        }
        [Authorize(Policy = "ViewProductReportPolicy")]
        [HttpGet]
        public async Task<IActionResult> ProductReport(DateTime? startDate, DateTime? endDate)
        {
            var model = await _productService.GetProductsAsync(startDate, endDate);
            return View(model);
        }
        [Authorize(Policy = "ExportProductReportPolicy")]
        [HttpGet]
        public async Task<IActionResult> ExportProductsToExcel(DateTime? startDate, DateTime? endDate)
        {
            // Call the asynchronous method and await the result
            var productReport = await _productService.GetProductsAsync(startDate, endDate);

            // Create an Excel package
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("ProductReport");

            // Add header
            worksheet.Cells[1, 1].Value = "ProductId";
            worksheet.Cells[1, 2].Value = "ProductName";
            worksheet.Cells[1, 3].Value = "ProductCode";
            worksheet.Cells[1, 4].Value = "CreatedAt";
            worksheet.Cells[1, 5].Value = "Parameters";

            // Add rows
            var rowIndex = 2;
            foreach (var product in productReport.Products)
            {
                var parameters = string.Join(";", product.Parameters.Select(p => $"{p.ParameterName} ({p.MinValue} - {p.MaxValue})"));
                worksheet.Cells[rowIndex, 1].Value = product.ProductId;
                worksheet.Cells[rowIndex, 2].Value = product.ProductName;
                worksheet.Cells[rowIndex, 3].Value = product.ProductCode;
                worksheet.Cells[rowIndex, 4].Value = product.CreatedAt.ToString("yyyy-MM-dd");
                worksheet.Cells[rowIndex, 5].Value = parameters;
                rowIndex++;
            }

            // Prepare file result
            var fileName = "ProductReport.xlsx";
            var fileContent = package.GetAsByteArray();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [Authorize(Policy = "ViewBatchStatusReportPolicy")]
        public async Task<IActionResult> BatchReport(DateTime? startDate, DateTime? endDate)
        {
            var model = await _batchService.GetBatchesAsync(startDate, endDate);
            return View(model);
        }
        [Authorize(Policy = "ExportBatchStatusReportPolicy")]
        [HttpGet]
        public async Task<IActionResult> ExportBatchesToExcel(DateTime? startDate, DateTime? endDate)
        {
            var batchReport = await _batchService.GetBatchesAsync(startDate, endDate);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("BatchReport");

            worksheet.Cells[1, 1].Value = "BatchId";
            worksheet.Cells[1, 2].Value = "BatchName";
            worksheet.Cells[1, 3].Value = "EquipmentName";
            worksheet.Cells[1, 4].Value = "ProductName";
            worksheet.Cells[1, 5].Value = "BatchStartTime";
            worksheet.Cells[1, 6].Value = "BatchEndTime";
            worksheet.Cells[1, 7].Value = "Comments";
            worksheet.Cells[1, 8].Value = "BatchStatus";
            worksheet.Cells[1, 9].Value = "Parameters";

            var rowIndex = 2;
            foreach (var batch in batchReport.Batches)
            {
                var parameters = string.Join(";", batch.BatchParameters.Select(p => $"{p.ParameterName} ({p.MinValue} - {p.MaxValue}): (Within range: {p.IsWithinRange})"));
                worksheet.Cells[rowIndex, 1].Value = batch.BatchId;
                worksheet.Cells[rowIndex, 2].Value = batch.BatchName;
                worksheet.Cells[rowIndex, 3].Value = batch.EquipmentName;
                worksheet.Cells[rowIndex, 4].Value = batch.ProductName;
                worksheet.Cells[rowIndex, 5].Value = batch.BatchStartTime.ToString("yyyy-MM-dd");
                worksheet.Cells[rowIndex, 6].Value = batch.BatchEndTime.ToString("yyyy-MM-dd");
                worksheet.Cells[rowIndex, 7].Value = batch.Comments;
                worksheet.Cells[rowIndex, 8].Value = batch.BatchStatus;
                worksheet.Cells[rowIndex, 9].Value = parameters;
                rowIndex++;
            }

            var fileName = "BatchReport.xlsx";
            var fileContent = package.GetAsByteArray();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [Authorize(Policy = "ExportUserActivityReportPolicy")]
        [HttpGet]
        public async Task<IActionResult> ExportUserActivitiesToExcel(DateTime? startDate, DateTime? endDate)
        {
            var userActivityReport = await _activityService.GetUserActivitiesAsync(startDate, endDate);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("UserActivityReport");

            worksheet.Cells[1, 1].Value = "Timestamp";
            worksheet.Cells[1, 2].Value = "User Id";
            worksheet.Cells[1, 3].Value = "Activity Type";
            worksheet.Cells[1, 4].Value = "Activity Description";

            var rowIndex = 2;
            foreach (var activity in userActivityReport.Activities)
            {
                worksheet.Cells[rowIndex, 1].Value = activity.Timestamp.ToString("MMM d, yyyy h:mm tt");
                worksheet.Cells[rowIndex, 2].Value = activity.UserName;
                worksheet.Cells[rowIndex, 3].Value = activity.ActivityType;
                worksheet.Cells[rowIndex, 4].Value = activity.ActivityDescription;
                rowIndex++;
            }

            var fileName = "UserActivityReport.xlsx";
            var fileContent = package.GetAsByteArray();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [Authorize(Policy = "ViewBackupReportPolicy")]
        [HttpGet]
        public async Task<IActionResult> BackupReport(DateTime? startDate, DateTime? endDate)
        {
            var backupLogs = await _backupService.GetBackupLogsAsync(startDate, endDate);
            var model = new BackupReportModel
            {
                StartDate = startDate,
                EndDate = endDate,
                BackupLogs = backupLogs
            };

            return View(model);
        }
        [Authorize(Policy = "ExportBackupReportPolicy")]
        [HttpGet]
        public async Task<IActionResult> ExportBackupToExcel(DateTime? startDate, DateTime? endDate)
        {
            // Fetch backup logs from the backup service
            var backupLogs = await _backupService.GetBackupLogsAsync(startDate, endDate);

            // Create an Excel package
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("BackupReport");

            // Add header
            worksheet.Cells[1, 1].Value = "EquipmentId";
            worksheet.Cells[1, 2].Value = "FolderName";
           
            worksheet.Cells[1, 4].Value = "LocalDestinationPath";
            worksheet.Cells[1, 5].Value = "RemoteDestinationPath";
            worksheet.Cells[1, 6].Value = "NumberOfUniqueBatches";
            worksheet.Cells[1, 7].Value = "NumberOfDuplicateBatches";
            worksheet.Cells[1, 8].Value = "TotalNumberOfBatches";
            worksheet.Cells[1, 9].Value = "BackupUser";
            worksheet.Cells[1, 10].Value = "BackupDate";

            // Add rows
            var rowIndex = 2;
            foreach (var log in backupLogs)
            {
                worksheet.Cells[rowIndex, 1].Value = log.EquipmentId;
                worksheet.Cells[rowIndex, 2].Value = log.FolderName;
               
                worksheet.Cells[rowIndex, 4].Value = log.LocalDestinationPath;
                worksheet.Cells[rowIndex, 5].Value = log.RemoteDestinationPath;
                worksheet.Cells[rowIndex, 6].Value = log.NumberOfUniqueBatches;
                worksheet.Cells[rowIndex, 7].Value = log.NumberOfDuplicateBatches;
                worksheet.Cells[rowIndex, 8].Value = log.TotalNumberOfBatches;
                worksheet.Cells[rowIndex, 9].Value = log.BackupUser;
                worksheet.Cells[rowIndex, 10].Value = log.BackupDate.ToString("yyyy-MM-dd HH:mm:ss");
                rowIndex++;
            }

            // Prepare file result
            var fileName = "BackupReport.xlsx";
            var fileContent = package.GetAsByteArray();
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }






    }
}

