using System.Net.Http.Json;
using DataGenerator.Fakers;

namespace DataGenerator;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _http;
    private readonly IConfiguration _config;
    private readonly CustomerFaker _customerFaker = new();
    private readonly OrderFaker _orderFaker = new();

    public Worker(ILogger<Worker> logger, HttpClient http, IConfiguration config)
    {
        _logger = logger;
        _http = http;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalSeconds = _config.GetValue("DataGenerator:IntervalSeconds", 30);
        var customerBatchSize = _config.GetValue("DataGenerator:CustomerBatchSize", 2);
        var orderBatchSize = _config.GetValue("DataGenerator:OrderBatchSize", 5);

        // One-time bulk seed mode
        var bulkCustomers = _config.GetValue("DataGenerator:BulkCustomers", 0);
        var bulkOrders = _config.GetValue("DataGenerator:BulkOrders", 0);

        _logger.LogInformation("Data Generator starting — interval: {Interval}s, customers/batch: {Customers}, orders/batch: {Orders}",
            intervalSeconds, customerBatchSize, orderBatchSize);

        // Wait for the API to be ready
        await WaitForApiAsync(stoppingToken);

        // Run bulk seed if configured, then continue with normal loop
        if (bulkCustomers > 0 || bulkOrders > 0)
        {
            await BulkSeedAsync(bulkCustomers, bulkOrders, stoppingToken);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 1. Fetch existing products (seeded in Phase 1)
                var products = await _http.GetFromJsonAsync<List<ProductResponse>>("api/products", stoppingToken) ?? [];
                if (products.Count == 0)
                {
                    _logger.LogWarning("No products found — skipping this cycle");
                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
                    continue;
                }

                var productInfos = products.Select(p => new ProductInfo(p.Id, p.Price)).ToList();

                // 2. Create customers
                var newCustomerIds = new List<int>();
                var fakeCustomers = _customerFaker.Generate(customerBatchSize);

                foreach (var customer in fakeCustomers)
                {
                    var response = await _http.PostAsJsonAsync("api/customers", customer, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var created = await response.Content.ReadFromJsonAsync<CustomerResponse>(cancellationToken: stoppingToken);
                        if (created is not null)
                        {
                            newCustomerIds.Add(created.Id);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create customer: {Status}", response.StatusCode);
                    }
                }

                _logger.LogInformation("Created {Count} customers", newCustomerIds.Count);

                // Also fetch existing customers to use as order sources
                var allCustomers = await _http.GetFromJsonAsync<List<CustomerResponse>>("api/customers", stoppingToken) ?? [];
                var allCustomerIds = allCustomers.Select(c => c.Id).ToList();

                if (allCustomerIds.Count == 0)
                {
                    _logger.LogWarning("No customers available for orders — skipping");
                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
                    continue;
                }

                // 3. Create orders
                var random = new Random();
                var ordersCreated = 0;

                for (var i = 0; i < orderBatchSize; i++)
                {
                    var customerId = allCustomerIds[random.Next(allCustomerIds.Count)];
                    var order = _orderFaker.Generate(customerId, productInfos);

                    var response = await _http.PostAsJsonAsync("api/orders", order, stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        ordersCreated++;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create order: {Status}", response.StatusCode);
                    }
                }

                _logger.LogInformation("Created {Count} orders", ordersCreated);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error during data generation cycle");
            }

            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), stoppingToken);
        }
    }

    private async Task BulkSeedAsync(int customerCount, int orderCount, CancellationToken ct)
    {
        _logger.LogInformation("Starting bulk seed: {Customers} customers, {Orders} orders", customerCount, orderCount);

        // Fetch products
        var products = await _http.GetFromJsonAsync<List<ProductResponse>>("api/products", ct) ?? [];
        if (products.Count == 0)
        {
            _logger.LogWarning("No products found — cannot bulk seed");
            return;
        }
        var productInfos = products.Select(p => new ProductInfo(p.Id, p.Price)).ToList();

        // Create customers in batches of 50
        var createdCustomerIds = new List<int>();
        var fakeCustomers = _customerFaker.Generate(customerCount);
        var customerBatch = 0;

        foreach (var chunk in fakeCustomers.Chunk(50))
        {
            foreach (var customer in chunk)
            {
                if (ct.IsCancellationRequested) return;
                var response = await _http.PostAsJsonAsync("api/customers", customer, ct);
                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<CustomerResponse>(cancellationToken: ct);
                    if (created is not null) createdCustomerIds.Add(created.Id);
                }
            }
            customerBatch += chunk.Length;
            _logger.LogInformation("Bulk seed: created {Count}/{Total} customers", customerBatch, customerCount);
        }

        // Fetch all customer IDs for order creation
        var allCustomers = await _http.GetFromJsonAsync<List<CustomerResponse>>("api/customers", ct) ?? [];
        var allCustomerIds = allCustomers.Select(c => c.Id).ToList();

        if (allCustomerIds.Count == 0)
        {
            _logger.LogWarning("No customers available — skipping order creation");
            return;
        }

        // Pre-generate all orders, then sort by date so IDs are sequential chronologically
        var random = new Random();
        var ordersCreated = 0;

        var preGeneratedOrders = new List<OrderDto>();
        for (var i = 0; i < orderCount; i++)
        {
            var customerId = allCustomerIds[random.Next(allCustomerIds.Count)];
            preGeneratedOrders.Add(_orderFaker.Generate(customerId, productInfos, useRandomDate: true));
        }

        // Sort by date so auto-increment IDs follow chronological order
        preGeneratedOrders.Sort((a, b) => (a.OrderDate ?? DateTime.MaxValue).CompareTo(b.OrderDate ?? DateTime.MaxValue));
        _logger.LogInformation("Bulk seed: pre-generated {Count} orders, posting in chronological order", preGeneratedOrders.Count);

        foreach (var order in preGeneratedOrders)
        {
            if (ct.IsCancellationRequested) return;

            var response = await _http.PostAsJsonAsync("api/orders", order, ct);
            if (response.IsSuccessStatusCode)
            {
                ordersCreated++;
            }
            else
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Order creation failed ({Status}): {Body}", response.StatusCode, body.Length > 200 ? body[..200] : body);
                await Task.Delay(500, ct); // back off on errors
            }

            // Throttle to avoid overwhelming SQL Basic tier
            if (ordersCreated % 10 == 0)
                await Task.Delay(100, ct);

            if (ordersCreated % 100 == 0 && ordersCreated > 0)
            {
                _logger.LogInformation("Bulk seed: created {Count}/{Total} orders", ordersCreated, orderCount);
            }
        }

        _logger.LogInformation("Bulk seed complete: {Customers} customers, {Orders} orders", createdCustomerIds.Count, ordersCreated);
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
                _logger.LogWarning("Health check returned {Status}", response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Health check failed: {Error}", ex.Message);
            }

            _logger.LogInformation("Waiting for Orders API...");
            await Task.Delay(3000, stoppingToken);
        }
    }
}

// Response DTOs for deserialization
record CustomerResponse(int Id, string FirstName, string LastName);
record ProductResponse(int Id, string Name, decimal Price);
