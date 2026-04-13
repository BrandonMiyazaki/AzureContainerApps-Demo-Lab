using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Azure.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using OrdersApi.Data;
using OrdersApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// --- Database ---
var connectionString = builder.Configuration.GetConnectionString("RetailDb");
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<RetailDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<RetailDbContext>(options =>
        options.UseAzureSql(connectionString));
}

// --- Authentication (Entra ID) — only enforce when configured ---
var azureAdSection = builder.Configuration.GetSection("AzureAd");
var isAuthConfigured = !string.IsNullOrEmpty(azureAdSection["TenantId"])
    && azureAdSection["TenantId"] != "<your-tenant-id>";

if (isAuthConfigured)
{
    builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
}
else
{
    builder.Services.AddAuthentication();
}

builder.Services.AddAuthorization();

// --- Rate Limiting ---
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

// --- Auto-migrate on startup ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RetailDbContext>();
    db.Database.Migrate();
}

// --- Middleware pipeline ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// --- Health Check ---
app.MapGet("/healthz", () => Results.Ok("Healthy"))
    .AllowAnonymous();

// --- Customer Endpoints ---
app.MapGet("/api/customers", async (RetailDbContext db) =>
    await db.Customers.AsNoTracking().ToListAsync())
    .RequireRateLimiting("fixed");

app.MapPost("/api/customers", async (RetailDbContext db, Customer customer) =>
{
    customer.CreatedAt = DateTime.UtcNow;
    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    return Results.Created($"/api/customers/{customer.Id}", customer);
}).RequireRateLimiting("fixed");

// --- Product Endpoints ---
app.MapGet("/api/products", async (RetailDbContext db) =>
    await db.Products.AsNoTracking().ToListAsync())
    .RequireRateLimiting("fixed");

app.MapPost("/api/products", async (RetailDbContext db, Product product) =>
{
    product.CreatedAt = DateTime.UtcNow;
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
}).RequireRateLimiting("fixed");

app.MapPut("/api/products/{id}/stock", async (RetailDbContext db, int id, StockUpdateRequest request) =>
{
    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();
    product.StockQuantity = request.StockQuantity;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireRateLimiting("fixed");

// --- Order Endpoints ---
app.MapGet("/api/orders", async (RetailDbContext db, DateTime? since) =>
{
    var query = db.Orders
        .Include(o => o.Customer)
        .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
        .AsNoTracking();

    if (since.HasValue)
    {
        query = query.Where(o => o.OrderDate > since.Value);
    }

    return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
}).RequireRateLimiting("fixed");

app.MapPost("/api/orders", async (RetailDbContext db, CreateOrderRequest request) =>
{
    var order = new Order
    {
        CustomerId = request.CustomerId,
        OrderDate = request.OrderDate ?? DateTime.UtcNow,
        OrderItems = request.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice
        }).ToList()
    };
    order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);

    db.Orders.Add(order);
    await db.SaveChangesAsync();
    return Results.Created($"/api/orders/{order.Id}", order);
}).RequireRateLimiting("fixed");

// --- Admin Endpoints (Development only) ---
if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/admin/randomize-dates", async (RetailDbContext db) =>
    {
        var updated = await db.Database.ExecuteSqlRawAsync(
            "UPDATE Orders SET OrderDate = DATEADD(SECOND, ABS(CHECKSUM(NEWID())) % 315360000, '2016-01-01')");
        return Results.Ok(new { updated });
    }).AllowAnonymous();
}

app.Run();

// --- Request DTOs ---
record StockUpdateRequest(int StockQuantity);
record CreateOrderRequest(int CustomerId, List<CreateOrderItemRequest> Items, DateTime? OrderDate = null);
record CreateOrderItemRequest(int ProductId, int Quantity, decimal UnitPrice);
