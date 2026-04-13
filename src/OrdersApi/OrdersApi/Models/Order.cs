namespace OrdersApi.Models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    public Customer Customer { get; set; } = null!;
    public List<OrderItem> OrderItems { get; set; } = [];
}
