using Azure.Identity;
using DataGenerator;

var builder = Host.CreateApplicationBuilder(args);

var apiBaseUrl = builder.Configuration["API_BASE_URL"];
if (string.IsNullOrEmpty(apiBaseUrl))
    apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL");
if (string.IsNullOrEmpty(apiBaseUrl))
    apiBaseUrl = "http://localhost:5000";

Console.WriteLine($"DataGenerator starting with API_BASE_URL: {apiBaseUrl}");

builder.Services.AddHttpClient("orders-api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddSingleton(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("orders-api");
});

// In Azure, acquire Entra ID token for API auth
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton(new DefaultAzureCredential());
}

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
