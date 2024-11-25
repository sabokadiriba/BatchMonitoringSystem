using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BatchMonitoringSystem.Controllers
{
    public class CreateProduct
    {
        private readonly ApplicationDbContext _context;
        public CreateProduct(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateProductAsync(ProductDto productDto)
        {
            var product = new Product
            {
                ProductName = productDto.ProductName,
                ProductCode = productDto.ProductCode,
                CreatedAt = DateTime.Now,
                Parameters = productDto.Parameters.Select(p => new ProductParameter
                {
                    ParameterName = p.ParameterName,
                    MinValue = p.MinValue,
                    MaxValue = p.MaxValue
                }).ToList()
            };
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }



        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var existingProduct = await _context.Products
                .Include(p => p.Parameters)
                .FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId);

            if (existingProduct == null)
            {
                // Handle not found
                throw new KeyNotFoundException("Product not found.");
            }

            existingProduct.ProductName = productDto.ProductName;
            existingProduct.ProductCode = productDto.ProductCode;

            // Remove existing parameters
            _context.ProductParameters.RemoveRange(existingProduct.Parameters);

            // Add updated parameters
            existingProduct.Parameters = productDto.Parameters.Select(p => new ProductParameter
            {
                ParameterName = p.ParameterName,
                MinValue = p.MinValue,
                MaxValue = p.MaxValue,
                ProductId = existingProduct.ProductId // Ensure correct ProductId is set
            }).ToList();

            await _context.SaveChangesAsync();
        }
    }
}
