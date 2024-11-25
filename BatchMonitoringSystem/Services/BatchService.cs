using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.Services;
using BatchMonitoringSystem.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
public class BatchService 
{
    private readonly ApplicationDbContext _context;
    private readonly IActualDataDbContextFactory _actualDataDbContextFactory;
    private readonly ILogger<ApplicationUser> _logger;
    public BatchService(ApplicationDbContext context, ILogger<ApplicationUser> logger, IActualDataDbContextFactory actualDataDbContextFactory)
    {
        _context = context;
        _logger = logger;
        _actualDataDbContextFactory = actualDataDbContextFactory;
    }

    public async Task<List<Batch>> GetAllBatchesAsync()
    {
        return await _context.Batches
            .Include(p=>p.Product)
            .Include(e=>e.Equipment)
           
            .ToListAsync();
    }

    public async Task<Batch> GetBatchAsync(int batchId)
    {
        return await _context.Batches
                     .Include(bp=>bp.BatchParameters)
                    .Include(p => p.Product)
                    .Include(e => e.Equipment)         
                    .FirstOrDefaultAsync(b => b.BatchId == batchId);

    }
  

public async Task<BatchReportModel> GetBatchesAsync(DateTime? startDate, DateTime? endDate)
{
    var batchesQuery = _context.Batches
        .Include(b => b.BatchParameters)
        .Include(b => b.Product)
        .Include(b => b.Equipment)
        .AsQueryable();

    if (startDate.HasValue)
    {
        batchesQuery = batchesQuery.Where(b => b.BatchStartTime >= startDate.Value);
    }

    if (endDate.HasValue)
    {
        batchesQuery = batchesQuery.Where(b => b.BatchEndTime <= endDate.Value);
    }

    var batches = await batchesQuery
        .Select(b => new BatchDto
        {
            BatchId = b.BatchId,
            BatchName = b.BatchName,
            EquipmentName = b.Equipment.EquipmentName,
            ProductName = b.Product.ProductName,
            BatchStartTime = b.BatchStartTime,
            BatchEndTime = b.BatchEndTime,
            Comments = b.Comments,
            BatchStatus = b.BatchStatus.ToString(),
            BatchParameters = b.BatchParameters.Select(bp => new BatchParameterDto
            {
                ParameterName = bp.ParameterName,
                // Deserialize JSON to List<double>
                ActualValues = string.IsNullOrEmpty(bp.ActualValuesJson)
                    ? new List<double>()
                    : JsonConvert.DeserializeObject<List<double>>(bp.ActualValuesJson),
                MinValue = bp.MinValue,
                MaxValue = bp.MaxValue,
                IsWithinRange = bp.IsWithinRange
            }).ToList()
        })
        .ToListAsync();

    return new BatchReportModel
    {
        StartDate = startDate,
        EndDate = endDate,
        Batches = batches
    };
}
public async Task<List<SelectListItem>> GetProductSelectListAsync()
    {
        return await _context.Products
            .Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.ProductName
            })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> GetEquipmentSelectListAsync()
    {
        return await _context.Equipment
            .Select(e => new SelectListItem
            {
                Value = e.EquipmentId.ToString(),
                Text = e.EquipmentName
            })
            .ToListAsync();
    }

 

    public async Task<bool> UpdateBatchStatusAsync(int batchId)
{
    // Retrieve the batch and related product
    var batch = await _context.Batches
        .Include(b => b.Equipment)
        .Include(b => b.Product)
        .FirstOrDefaultAsync(b => b.BatchId == batchId);

    if (batch == null) return false;

    var product = await _context.Products
        .Include(p => p.Parameters)
        .FirstOrDefaultAsync(p => p.ProductId == batch.ProductId);

    if (product == null) return false;

    // Create ActualDataDbContext dynamically based on equipment
     var actualDataDbContext =_actualDataDbContextFactory.CreateDbContext(batch.Equipment);
        var testData = await actualDataDbContext.tblActualData.ToListAsync();
        Console.WriteLine($"Records count: {testData.Count}");
        // Retrieve actual data records for the batch
        var actualDataRecords = await actualDataDbContext.tblActualData
        .Where(ad => ad.BatchId == 4005)
        .ToListAsync();

    // Retrieve existing BatchParameter records
    var existingBatchParameters = await _context.BatchParameters
        .Where(bp => bp.BatchId == 1)
        .ToListAsync();

    bool allParametersWithinRange = true;
    bool hasAcceptableParameters = false;

    // Process each parameter
    foreach (var param in product.Parameters)
    {
        // Get all actual values for the current parameter
        var actualParams = actualDataRecords
            .Where(ad => ad.ParameterName == param.ParameterName)
            .Select(ad => ad.ActualValue)
            .ToList();

        // If no actual values, skip
        if (!actualParams.Any())
        {
            continue;
        }

        // Convert the list of actual values to JSON
        var actualValuesJson = JsonConvert.SerializeObject(actualParams);

        bool isWithinRange = actualParams.All(value => value >= param.MinValue && value <= param.MaxValue);

        // Find if there is an existing BatchParameter record
        var batchParameter = existingBatchParameters
            .FirstOrDefault(bp => bp.ParameterName == param.ParameterName);

        if (batchParameter != null)
        {
            // Update the existing BatchParameter record
            batchParameter.ActualValuesJson = actualValuesJson;
            batchParameter.MinValue = param.MinValue;
            batchParameter.MaxValue = param.MaxValue;
            batchParameter.IsWithinRange = isWithinRange;
            batchParameter.Comment = isWithinRange ? null : "Out of range";
        }
        else
        {
            // Create a new BatchParameter record if it doesn't exist
            batchParameter = new BatchParameter
            {
                BatchId = batchId,
                ParameterName = param.ParameterName,
                ActualValuesJson = actualValuesJson,
                MinValue = param.MinValue,
                MaxValue = param.MaxValue,
                IsWithinRange = isWithinRange,
                Comment = isWithinRange ? null : "Out of range"
            };

            _context.BatchParameters.Add(batchParameter);
        }

        if (!isWithinRange)
        {
            allParametersWithinRange = false;
            hasAcceptableParameters = true;
        }
    }

    // Update the batch status
    if (allParametersWithinRange)
    {
        batch.BatchStatus = BatchStatus.Passed;
        batch.Comments = "Batch passed successfully.";
    }
    else if (hasAcceptableParameters)
    {
        batch.BatchStatus = BatchStatus.Failed;
        batch.Comments = "Parameters out of range.";
    }
    else
    {
        batch.BatchStatus = BatchStatus.Failed;
        batch.Comments = "Parameters are out of range and not acceptable.";
    }

    // Save changes to the database
    await _context.SaveChangesAsync();
    return true;
}


}
