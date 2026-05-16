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
    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var query = from p in _context.Products
                    join pp in _context.ProductPrices on p.ProductId equals pp.PriceProductId into ppJoin
                    from ppResult in ppJoin.DefaultIfEmpty()
                    join ds in _context.DataStocks on p.ProductId equals ds.IdProduct into dsJoin
                    where dsJoin.Sum(x => x.Quantity) > 0
                    select new ProductDto
                    {
                        ProductId = p.ProductId,
                        ProductCode = p.ProductCode,
                        ProductName1 = p.ProductName1,
                        ProductName2 = p.ProductName2,
                        CostValue = p.CostValue,
                        SellingPrice = (decimal?)ppResult.PriceProduct,
                        Quantity = (double?)dsJoin.Sum(x => x.Quantity)
                    };

        return await query.ToListAsync();
    }

    /// <summary>
    /// Get product by exact code
    /// </summary>
    public async Task<ProductDto?> GetProductByCodeAsync(string code)
    {
        var normalizedCode = code.Replace("-", "").Replace(" ", "");
        var query = from p in _context.Products.Where(p => p.ProductCode != null && p.ProductCode.Replace("-", "").Replace(" ", "") == normalizedCode)
                    join pp in _context.ProductPrices on p.ProductId equals pp.PriceProductId into ppJoin
                    from ppResult in ppJoin.DefaultIfEmpty()
                    join ds in _context.DataStocks on p.ProductId equals ds.IdProduct into dsJoin
                    where dsJoin.Sum(x => x.Quantity) > 0
                    select new ProductDto
                    {
                        ProductId = p.ProductId,
                        ProductCode = p.ProductCode,
                        ProductName1 = p.ProductName1,
                        ProductName2 = p.ProductName2,
                        CostValue = p.CostValue,
                        SellingPrice = (decimal?)ppResult.PriceProduct,
                        Quantity = (double?)dsJoin.Sum(x => x.Quantity)
                    };

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Search products by code or name (partial match)
    /// </summary>
    public async Task<List<ProductDto>> SearchProductsAsync(string queryText)
    {
        if (string.IsNullOrWhiteSpace(queryText))
            return new List<ProductDto>();

        var lowerQuery = queryText.ToLower();
        var normalizedQuery = queryText.Replace("-", "").Replace(" ", "").ToLower();
        
        var query = from p in _context.Products
                    where (p.ProductCode != null && p.ProductCode.Replace("-", "").Replace(" ", "").ToLower().Contains(normalizedQuery)) 
                       || (p.ProductName1 != null && p.ProductName1.ToLower().Contains(lowerQuery))
                       || (p.ProductName2 != null && p.ProductName2.ToLower().Contains(lowerQuery))
                    join pp in _context.ProductPrices on p.ProductId equals pp.PriceProductId into ppJoin
                    from ppResult in ppJoin.DefaultIfEmpty()
                    join ds in _context.DataStocks on p.ProductId equals ds.IdProduct into dsJoin
                    where dsJoin.Sum(x => x.Quantity) > 0
                    select new ProductDto
                    {
                        ProductId = p.ProductId,
                        ProductCode = p.ProductCode,
                        ProductName1 = p.ProductName1,
                        ProductName2 = p.ProductName2,
                        CostValue = p.CostValue,
                        SellingPrice = (decimal?)ppResult.PriceProduct,
                        Quantity = (double?)dsJoin.Sum(x => x.Quantity)
                    };

        return await query.ToListAsync();
    }
}
