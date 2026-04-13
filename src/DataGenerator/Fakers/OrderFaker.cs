using Bogus;

namespace DataGenerator.Fakers;

public record OrderItemDto(int ProductId, int Quantity, decimal UnitPrice);
public record OrderDto(int CustomerId, List<OrderItemDto> Items, DateTime? OrderDate = null);

public class OrderFaker
{
    private readonly Faker _faker = new();

    /// <summary>
    /// Generate an order with an optional random date.
    /// When useRandomDate is true, the order date is set to a random date
    /// within the last 10 years (from April 13, 2016 to April 13, 2026).
    /// </summary>
    public OrderDto Generate(int customerId, List<ProductInfo> availableProducts, bool useRandomDate = false)
    {
        var itemCount = _faker.Random.Int(1, 4);
        var selectedProducts = _faker.PickRandom(availableProducts, itemCount).ToList();

        var items = selectedProducts.Select(p => new OrderItemDto(
            p.Id,
            _faker.Random.Int(1, 3),
            p.Price
        )).ToList();

        DateTime? orderDate = useRandomDate
            ? _faker.Date.Between(new DateTime(2016, 4, 13), new DateTime(2026, 4, 13))
            : null;

        return new OrderDto(customerId, items, orderDate);
    }
}

public record ProductInfo(int Id, decimal Price);
