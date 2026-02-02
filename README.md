# CFTClient - Products Wrapper API

A lightweight ASP.NET Core Minimal API that acts as a wrapper between SQL Server database and Cloudflare Tunnel for querying car parts products.

## Features

- ✅ RESTful API for product queries
- ✅ Search by product code or name
- ✅ API Key authentication
- ✅ Arabic RTL Bootstrap 5 UI
- ✅ In-memory database for testing
- ✅ SQL Server with Windows Authentication support

## Quick Start

### Development/Testing Mode

```bash
cd CFTClient
dotnet run
```

Open browser: http://localhost:5050

### Production Mode

1. Edit `appsettings.json`:
```json
{
  "TestMode": {
    "Enabled": false
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=YOUR_DB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

2. Run the application:
```bash
dotnet run
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products |
| `GET` | `/api/products/{code}` | Get product by code |
| `GET` | `/api/products/search?q={query}` | Search products |
| `GET` | `/health` | Health check (no auth) |

## Authentication

All `/api/*` endpoints require the `X-API-Key` header:

```bash
curl -H "X-API-Key: YOUR_API_KEY" http://localhost:5050/api/products
```

## Database Schema

```sql
CREATE TABLE products (
    product_id INT PRIMARY KEY,
    product_code VARCHAR(50),
    product_name NVARCHAR(255),
    Quantity INT,
    price_product DECIMAL(18,2)
);
```

## Cloudflare Tunnel Integration

```bash
cloudflared tunnel --url http://localhost:5050
```

## License

MIT
