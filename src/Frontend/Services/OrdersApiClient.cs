using Frontend.Models;

namespace Frontend.Services;

public class OrdersApiClient
{
    private readonly HttpClient _http;

    public OrdersApiClient(HttpClient http)
    {
        _http = http;
    }

    // Customers
    public async Task<List<Customer>> GetCustomersAsync()
        => await _http.GetFromJsonAsync<List<Customer>>("api/customers") ?? [];

    // Products
    public async Task<List<Product>> GetProductsAsync()
        => await _http.GetFromJsonAsync<List<Product>>("api/products") ?? [];

    // Orders
    public async Task<List<Order>> GetOrdersAsync()
        => await _http.GetFromJsonAsync<List<Order>>("api/orders") ?? [];

    public async Task<Order?> CreateOrderAsync(CreateOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/orders", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Order>();
    }
}

public class CreateOrderRequest
{
    public int CustomerId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
