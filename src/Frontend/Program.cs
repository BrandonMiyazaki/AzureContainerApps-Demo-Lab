using System.Globalization;
using Frontend.Components;
using Frontend.Services;

// Ensure currency formatting uses $ (en-US) regardless of container locale
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Typed HttpClient for the Orders API
var apiBaseUrl = builder.Configuration["API_BASE_URL"]
    ?? Environment.GetEnvironmentVariable("API_BASE_URL")
    ?? "http://localhost:5000";

builder.Services.AddHttpClient<OrdersApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Security headers (CSP + anti-clickjacking)
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self' ws: wss:;";
    await next();
});

app.UseAntiforgery();

// Health check
app.MapGet("/healthz", () => Results.Ok("Healthy"));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
