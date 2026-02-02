using Microsoft.EntityFrameworkCore;
using CFTClient.Data;
using CFTClient.Services;
using CFTClient.Middleware;
using CFTClient.Models;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// اختيار وضع الاختبار أو الإنتاج
// ========================================
var useTestMode = builder.Configuration.GetValue<bool>("TestMode:Enabled", true);

if (useTestMode)
{
    // وضع الاختبار - قاعدة بيانات في الذاكرة
    Console.WriteLine("🧪 وضع الاختبار: استخدام قاعدة بيانات In-Memory");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestProductsDb"));
}
else
{
    // وضع الإنتاج - SQL Server
    Console.WriteLine("🔗 وضع الإنتاج: الاتصال بـ SQL Server");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<ProductService>();

// Add CORS for Cloudflare Tunnel
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========================================
// إضافة بيانات تجريبية في وضع الاختبار
// ========================================
if (useTestMode)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { ProductId = 1, ProductCode = "FLT-001", ProductName = "فلتر هواء تويوتا كامري", Quantity = 25, PriceProduct = 85.00m },
                new Product { ProductId = 2, ProductCode = "FLT-002", ProductName = "فلتر زيت هيونداي النترا", Quantity = 40, PriceProduct = 45.00m },
                new Product { ProductId = 3, ProductCode = "BRK-001", ProductName = "طقم فحمات فرامل أمامية نيسان", Quantity = 15, PriceProduct = 180.00m },
                new Product { ProductId = 4, ProductCode = "BRK-002", ProductName = "طقم فحمات فرامل خلفية تويوتا", Quantity = 12, PriceProduct = 150.00m },
                new Product { ProductId = 5, ProductCode = "SPK-001", ProductName = "شمعات إشعال NGK", Quantity = 100, PriceProduct = 35.00m },
                new Product { ProductId = 6, ProductCode = "OIL-001", ProductName = "زيت محرك موبيل 1 5W-30", Quantity = 50, PriceProduct = 120.00m },
                new Product { ProductId = 7, ProductCode = "BAT-001", ProductName = "بطارية AC Delco 70 أمبير", Quantity = 8, PriceProduct = 450.00m },
                new Product { ProductId = 8, ProductCode = "BLT-001", ProductName = "سير مروحة كيا سيراتو", Quantity = 20, PriceProduct = 65.00m },
                new Product { ProductId = 9, ProductCode = "RAD-001", ProductName = "رديتر مياه هوندا أكورد", Quantity = 5, PriceProduct = 550.00m },
                new Product { ProductId = 10, ProductCode = "SUS-001", ProductName = "مساعد أمامي مازدا 3", Quantity = 0, PriceProduct = 320.00m }
            );
            context.SaveChanges();
            Console.WriteLine("✅ تم إضافة 10 منتجات تجريبية");
        }
    }
}

// Configure middleware
app.UseCors();
app.UseStaticFiles();
app.UseApiKeyMiddleware();

// Default route - serve index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

// API Endpoints
app.MapGet("/api/products", async (ProductService service) =>
{
    var products = await service.GetAllProductsAsync();
    return Results.Ok(new 
    { 
        success = true, 
        count = products.Count, 
        data = products 
    });
});

app.MapGet("/api/products/{code}", async (string code, ProductService service) =>
{
    var product = await service.GetProductByCodeAsync(code);
    if (product == null)
        return Results.NotFound(new { success = false, error = "Product not found" });
    
    return Results.Ok(new { success = true, data = product });
});

app.MapGet("/api/products/search", async (string? q, ProductService service) =>
{
    if (string.IsNullOrWhiteSpace(q))
        return Results.BadRequest(new { success = false, error = "Search query is required" });
    
    var products = await service.SearchProductsAsync(q);
    return Results.Ok(new 
    { 
        success = true, 
        query = q,
        count = products.Count, 
        data = products 
    });
});

// Health check endpoint (no auth required)
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

Console.WriteLine("===========================================");
Console.WriteLine("  CFTClient - Products Wrapper API");
Console.WriteLine("===========================================");
Console.WriteLine($"  API running on: http://localhost:5050");
Console.WriteLine($"  UI available at: http://localhost:5050/index.html");
Console.WriteLine("===========================================");

app.Run("http://localhost:5050");

