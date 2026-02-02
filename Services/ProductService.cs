using Microsoft.EntityFrameworkCore;
using CFTClient.Data;
using CFTClient.Models;

namespace CFTClient.Services;

public class ProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    /// <summary>
    /// Get product by exact code
    /// </summary>
    public async Task<Product?> GetProductByCodeAsync(string code)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.ProductCode == code);
    }

    /// <summary>
    /// Search products by code or name (partial match)
    /// </summary>
    public async Task<List<Product>> SearchProductsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new List<Product>();

        var lowerQuery = query.ToLower();
        
        return await _context.Products
            .Where(p => p.ProductCode.ToLower().Contains(lowerQuery) 
                     || p.ProductName.ToLower().Contains(lowerQuery))
            .ToListAsync();
    }
}
