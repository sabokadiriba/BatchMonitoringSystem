using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using Microsoft.AspNetCore.Mvc;
using BatchMonitoringSystem.ViewModels;
using BatchMonitoringSystem.Services;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ProductService _productService;
    public ProductController(ApplicationDbContext context,ProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    [Authorize(Policy = "ViewProductPolicy")]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    [Authorize(Policy = "CreateProductPolicy")]
    [HttpGet]
    public IActionResult CreateProduct()
    {
        var model = new ProductDto();
       
        model.Parameters.Add(new ProductParameterDto()); // Add one parameter by default
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(ProductDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = new Product
        {  
            ProductName = model.ProductName,
            ProductCode = model.ProductCode,
            Parameters = model.Parameters.Select(p => new ProductParameter
            {
                ParameterName = p.ParameterName,
                MinValue = p.MinValue,
                MaxValue = p.MaxValue
            }).ToList()
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return RedirectToAction("CreateProduct"); // Redirect to the product list or details page
    }

    [Authorize(Policy = "EditProductPolicy")]
    [HttpGet]
    public async Task<IActionResult> UpdateProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(); // Handle not found
        }

        var model = new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductCode = product.ProductCode,
            Parameters = product.Parameters.Select(p => new ProductParameterDto
            {
                ParameterName = p.ParameterName,
                MinValue = p.MinValue,
                MaxValue = p.MaxValue
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(productDto);
            return RedirectToAction("Index"); // Redirect to a list or details page
        }
        return View(productDto);
    }


}
