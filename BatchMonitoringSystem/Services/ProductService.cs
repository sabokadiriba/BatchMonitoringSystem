using System.Linq;
using System.Threading.Tasks;
using BatchMonitoringSystem.Data;
using BatchMonitoringSystem.Models;
using BatchMonitoringSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BatchMonitoringSystem.Services
{
  
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Parameters) // Include the related ProductParameters
                .ToListAsync(); // Retrieve all products as a list
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Parameters)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return null;
            }

            return new ProductDto
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

        // Other service methods


        public async Task<ProductReportModel> GetProductsAsync(DateTime? startDate, DateTime? endDate)
        {
            var productsQuery = _context.Products.Include(p => p.Parameters).AsQueryable();

            if (startDate.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CreatedAt <= endDate.Value);
            }

            var products = await productsQuery
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductCode = p.ProductCode,
                    CreatedAt = p.CreatedAt,
                    Parameters = p.Parameters.Select(param => new ProductParameterDto
                    {
                        ParameterName = param.ParameterName,
                        MinValue = param.MinValue,
                        MaxValue = param.MaxValue
                    }).ToList()
                })
                .ToListAsync();

            return new ProductReportModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Products = products
            };
        }



    }

}
