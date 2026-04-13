using Azure.Identity;
using InventoryService;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["API_BASE_URL"]
    ?? Environment.GetEnvironmentVariable("API_BASE_URL")
    ?? "http://localhost:5000";

builder.Services.AddHttpClient<Worker>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// In Azure, acquire Entra ID token for API auth
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton(new DefaultAzureCredential());
}

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Debug endpoint — view current in-memory stock ledger
app.MapGet("/api/inventory", (IEnumerable<IHostedService> services) =>
{
    var worker = services.OfType<Worker>().FirstOrDefault();
    return worker is not null
        ? Results.Ok(worker.StockLedger)
        : Results.NotFound("Inventory worker not found");
});

app.Run();
