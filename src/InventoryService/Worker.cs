using System.Collections.Concurrent;
using System.Net.Http.Json;

namespace InventoryService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly ConcurrentDictionary<int, int> _stockLedger = new();
    private DateTime _lastChecked = DateTime.MinValue;

    public Worker(ILogger<Worker> logger, HttpClient http, IConfiguration config)
    {
        _logger = logger;
        _http = http;
        _config = config;
    }

    public IReadOnlyDictionary<int, int> StockLedger => _stockLedger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _config.GetValue("Inventory:PollIntervalSeconds", 15);
        var lowStockThreshold = _config.GetValue("Inventory:LowStockThreshold", 10);

        _logger.LogInformation("Inventory Service starting — poll interval: {Interval}s, low-stock threshold: {Threshold}",
            intervalSeconds, lowStockThreshold);

        await WaitForApiAsync(stoppingToken);
        await InitializeStockLedgerAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var sinceParam = _lastChecked.ToString("o");
                var orders = await _http.GetFromJsonAsync<List<OrderResponse>>(
                    $"api/orders?since={Uri.EscapeDataString(sinceParam)}", stoppingToken) ?? [];

                if (orders.Count > 0)
                {
                    _logger.LogInformation("Processing {Count} new order(s)", orders.Count);

                    foreach (var order in orders)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            var newStock = _stockLedger.AddOrUpdate(
                                item.ProductId,
                                0,
                                (_, current) => Math.Max(0, current - item.Quantity));

                            // Persist to API
                            await _http.PutAsJsonAsync(
                                $"api/products/{item.ProductId}/stock",
                                new { StockQuantity = newStock },
                                stoppingToken);

                            if (newStock < lowStockThreshold)
                            {
                                _logger.LogWarning("LOW STOCK: Product {ProductId} has {Stock} units remaining",
                                    item.ProductId, newStock);
                            }
                        }

                        if (order.OrderDate > _lastChecked)
                        {
                            _lastChecked = order.OrderDate;
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error during inventory polling cycle");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }

    private async Task InitializeStockLedgerAsync(CancellationToken stoppingToken)
    {
        try
        {
            var products = await _http.GetFromJsonAsync<List<ProductResponse>>("api/products", stoppingToken) ?? [];
            foreach (var product in products)
            {
                _stockLedger[product.Id] = product.StockQuantity;
            }
            _logger.LogInformation("Initialized stock ledger with {Count} products", products.Count);

            // Set lastChecked to now so we only process future orders
            _lastChecked = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize stock ledger");
        }
    }

    private async Task WaitForApiAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await _http.GetAsync("healthz", stoppingToken);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Orders API is ready");
                    return;
                }
            }
            catch
            {
                // API not ready yet
            }

            _logger.LogInformation("Waiting for Orders API...");
            await Task.Delay(3000, stoppingToken);
        }
    }
}

record OrderResponse(int Id, DateTime OrderDate, List<OrderItemResponse> OrderItems);
record OrderItemResponse(int ProductId, int Quantity);
record ProductResponse(int Id, string Name, int StockQuantity);
