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
            var prices = new List<ProductPrice>
            {
                new ProductPrice { PriceProductId = 1, PriceProduct = 120.00m },
                new ProductPrice { PriceProductId = 2, PriceProduct = 60.00m },
                new ProductPrice { PriceProductId = 3, PriceProduct = 250.00m },
                new ProductPrice { PriceProductId = 4, PriceProduct = 200.00m },
                new ProductPrice { PriceProductId = 5, PriceProduct = 50.00m },
                new ProductPrice { PriceProductId = 6, PriceProduct = 160.00m },
                new ProductPrice { PriceProductId = 7, PriceProduct = 600.00m },
                new ProductPrice { PriceProductId = 8, PriceProduct = 90.00m },
                new ProductPrice { PriceProductId = 9, PriceProduct = 700.00m },
                new ProductPrice { PriceProductId = 10, PriceProduct = 450.00m }
            };
            context.ProductPrices.AddRange(prices);

            context.Products.AddRange(
                new Product { ProductId = 1, ProductCode = "FLT-001", ProductName1 = "فلتر هواء تويوتا كامري", ProductName2 = "Toyota Camry Air Filter", CostValue = 85.00d },
                new Product { ProductId = 2, ProductCode = "FLT-002", ProductName1 = "فلتر زيت هيونداي النترا", ProductName2 = "Hyundai Elantra Oil Filter", CostValue = 45.00d },
                new Product { ProductId = 3, ProductCode = "BRK-001", ProductName1 = "طقم فحمات فرامل أمامية نيسان", ProductName2 = "Nissan Front Brake Pads", CostValue = 180.00d },
                new Product { ProductId = 4, ProductCode = "BRK-002", ProductName1 = "طقم فحمات فرامل خلفية تويوتا", ProductName2 = "Toyota Rear Brake Pads", CostValue = 150.00d },
                new Product { ProductId = 5, ProductCode = "SPK-001", ProductName1 = "شمعات إشعال NGK", ProductName2 = "NGK Spark Plugs", CostValue = 35.00d },
                new Product { ProductId = 6, ProductCode = "OIL-001", ProductName1 = "زيت محرك موبيل 1 5W-30", ProductName2 = "Mobil 1 5W-30 Motor Oil", CostValue = 120.00d },
                new Product { ProductId = 7, ProductCode = "BAT-001", ProductName1 = "بطارية AC Delco 70 أمبير", ProductName2 = "AC Delco 70 Ah Battery", CostValue = 450.00d },
                new Product { ProductId = 8, ProductCode = "BLT-001", ProductName1 = "سير مروحة كيا سيراتو", ProductName2 = "Kia Cerato Fan Belt", CostValue = 65.00d },
                new Product { ProductId = 9, ProductCode = "RAD-001", ProductName1 = "رديتر مياه هوندا أكورد", ProductName2 = "Honda Accord Radiator", CostValue = 550.00d },
                new Product { ProductId = 10, ProductCode = "SUS-001", ProductName1 = "مساعد أمامي مازدا 3", ProductName2 = "Mazda 3 Front Shock Absorber", CostValue = 320.00d }
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

