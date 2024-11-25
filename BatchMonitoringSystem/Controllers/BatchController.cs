using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.ViewModels;
using BatchMonitoringSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BatchMonitoringSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

public class BatchController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly BatchService _batchService; // Assume you have a service class for batch operations

    public BatchController(ApplicationDbContext context, BatchService batchService)
    {
        _context = context;
        _batchService = batchService;
    }
    [Authorize(Policy = "ViewBatchPolicy")]
    public async Task<IActionResult> Index()
    {
        var batches = await _batchService.GetAllBatchesAsync();
        return View(batches);
    }
    [Authorize(Policy = "CreateBatchPolicy")]
    [HttpGet]
    public async Task<IActionResult> CreateBatch()
    {

        var ProductList = await _batchService.GetProductSelectListAsync();
        var EquipmentList = await _batchService.GetEquipmentSelectListAsync();
        ViewBag.ProductList = ProductList;
        ViewBag.EquipmentList = EquipmentList;
        return View();
    }
    [Authorize(Policy = "CreateBatchPolicy")]
    [HttpPost]
    public async Task<IActionResult> CreateBatch(BatchDto model)
    {
       
        var batch = new Batch
        {
            EquipmentId = model.EquipmentId,
            ProductId = model.ProductId,
            BatchStartTime = model.BatchStartTime,
            BatchEndTime = model.BatchEndTime,
            BatchName=model.BatchName
        };
        _context.Batches.Add(batch);
        await _context.SaveChangesAsync();
        return RedirectToAction("ViewBatch", new { batch.BatchId }); 
    }
    [Authorize(Policy = "ViewBatchPolicy")]
    [HttpGet]
    public async Task<IActionResult> ViewBatch(int BatchId)
    {  
        var batch = await _batchService.GetBatchAsync(BatchId);
        if (batch == null)
        {
            return NotFound();
        }
        return View(batch);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateBatchStatus(int batchId)
    {
        bool success = await _batchService.UpdateBatchStatusAsync(batchId);

        if (!success)
        {
            return NotFound();
        }
        return RedirectToAction("ViewBatch", new { batchId });
    }



    //=======================================================
    [Authorize(Policy = "MonitorBatchStatusPolicy")]

    public async Task<IActionResult> BatchMonitoring()
    {
        var viewModel = new BatchFilterViewModel
        {
            EquipmentList = await _context.Equipment
                .Select(e => new SelectListItem
                {
                    Value = e.EquipmentId.ToString(),
                    Text = e.EquipmentName
                })
                .ToListAsync(),

            ProductList = await _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = p.ProductName
                })
                .ToListAsync()
        };

        return View(viewModel);
    }



    [HttpGet]
    public async Task<IActionResult> GetFilteredBatches(int equipmentId, int productId)
    {
        var batches = await _context.Batches
            .Where(b => b.EquipmentId == equipmentId && b.ProductId == productId)
            .Select(b => new
            {
                Value = b.BatchId,
                Text = $"Batch ID: {b.BatchId} - Start: {b.BatchStartTime} - End: {b.BatchEndTime}"
            })
            .ToListAsync();

        return Json(batches);
    }

    private async Task<BatchFilterViewModel> GetBatchFilterViewModel(int selectedEquipmentId = 0, int selectedProductId = 0)
    {
        return new BatchFilterViewModel
        {
            EquipmentList = new SelectList(await _context.Equipment.ToListAsync(), "EquipmentId", "EquipmentName", selectedEquipmentId),
            ProductList = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName", selectedProductId),
            BatchList = new SelectList((System.Collections.IEnumerable)await GetFilteredBatches(selectedEquipmentId, selectedProductId), "Value", "Text"),
            SelectedEquipmentId = selectedEquipmentId,
            SelectedProductId = selectedProductId
        };
    }


}
