using Microsoft.EntityFrameworkCore;
using OrdersApi.Models;

namespace OrdersApi.Data;

public class RetailDbContext : DbContext
{
    public RetailDbContext(DbContextOptions<RetailDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");

        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", City = "Seattle", State = "WA", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", City = "Portland", State = "OR", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 3, FirstName = "Carol", LastName = "Williams", Email = "carol@example.com", City = "San Francisco", State = "CA", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 4, FirstName = "David", LastName = "Brown", Email = "david@example.com", City = "Denver", State = "CO", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Customer { Id = 5, FirstName = "Eva", LastName = "Davis", Email = "eva@example.com", City = "Austin", State = "TX", CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed Products (100 items across diverse retail categories)
        var seedDate = new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Product>().HasData(
            // Electronics (20)
            new Product { Id = 1, Name = "Wireless Mouse", Category = "Electronics", Price = 29.99m, StockQuantity = 150, CreatedAt = seedDate },
            new Product { Id = 2, Name = "Mechanical Keyboard", Category = "Electronics", Price = 89.99m, StockQuantity = 75, CreatedAt = seedDate },
            new Product { Id = 3, Name = "USB-C Hub", Category = "Electronics", Price = 49.99m, StockQuantity = 200, CreatedAt = seedDate },
            new Product { Id = 6, Name = "Webcam HD", Category = "Electronics", Price = 59.99m, StockQuantity = 90, CreatedAt = seedDate },
            new Product { Id = 11, Name = "27\" 4K Monitor", Category = "Electronics", Price = 349.99m, StockQuantity = 40, CreatedAt = seedDate },
            new Product { Id = 12, Name = "Portable SSD 1TB", Category = "Electronics", Price = 89.99m, StockQuantity = 120, CreatedAt = seedDate },
            new Product { Id = 13, Name = "Wireless Charger Pad", Category = "Electronics", Price = 19.99m, StockQuantity = 300, CreatedAt = seedDate },
            new Product { Id = 14, Name = "HDMI Cable 6ft", Category = "Electronics", Price = 9.99m, StockQuantity = 500, CreatedAt = seedDate },
            new Product { Id = 15, Name = "USB Flash Drive 64GB", Category = "Electronics", Price = 12.99m, StockQuantity = 400, CreatedAt = seedDate },
            new Product { Id = 16, Name = "Smart Power Strip", Category = "Electronics", Price = 34.99m, StockQuantity = 150, CreatedAt = seedDate },
            new Product { Id = 17, Name = "Tablet Stand", Category = "Electronics", Price = 24.99m, StockQuantity = 200, CreatedAt = seedDate },
            new Product { Id = 18, Name = "Laptop Docking Station", Category = "Electronics", Price = 149.99m, StockQuantity = 55, CreatedAt = seedDate },
            new Product { Id = 19, Name = "Ethernet Adapter USB-C", Category = "Electronics", Price = 18.99m, StockQuantity = 250, CreatedAt = seedDate },
            new Product { Id = 20, Name = "Wireless Presenter Remote", Category = "Electronics", Price = 29.99m, StockQuantity = 130, CreatedAt = seedDate },
            new Product { Id = 21, Name = "Surge Protector 8-Outlet", Category = "Electronics", Price = 22.99m, StockQuantity = 180, CreatedAt = seedDate },
            new Product { Id = 22, Name = "MicroSD Card 128GB", Category = "Electronics", Price = 17.99m, StockQuantity = 350, CreatedAt = seedDate },
            new Product { Id = 23, Name = "Portable Monitor 15.6\"", Category = "Electronics", Price = 199.99m, StockQuantity = 35, CreatedAt = seedDate },
            new Product { Id = 24, Name = "Cable Management Kit", Category = "Electronics", Price = 14.99m, StockQuantity = 270, CreatedAt = seedDate },
            new Product { Id = 25, Name = "Smart Plug 4-Pack", Category = "Electronics", Price = 29.99m, StockQuantity = 160, CreatedAt = seedDate },
            new Product { Id = 26, Name = "USB Microphone", Category = "Electronics", Price = 69.99m, StockQuantity = 85, CreatedAt = seedDate },

            // Audio (10)
            new Product { Id = 8, Name = "Noise-Cancelling Headphones", Category = "Audio", Price = 199.99m, StockQuantity = 60, CreatedAt = seedDate },
            new Product { Id = 9, Name = "Bluetooth Speaker", Category = "Audio", Price = 34.99m, StockQuantity = 180, CreatedAt = seedDate },
            new Product { Id = 27, Name = "Wireless Earbuds", Category = "Audio", Price = 79.99m, StockQuantity = 110, CreatedAt = seedDate },
            new Product { Id = 28, Name = "Studio Monitor Speakers", Category = "Audio", Price = 249.99m, StockQuantity = 25, CreatedAt = seedDate },
            new Product { Id = 29, Name = "Soundbar 2.1", Category = "Audio", Price = 159.99m, StockQuantity = 45, CreatedAt = seedDate },
            new Product { Id = 30, Name = "Podcast Starter Kit", Category = "Audio", Price = 129.99m, StockQuantity = 30, CreatedAt = seedDate },
            new Product { Id = 31, Name = "Audio Interface USB", Category = "Audio", Price = 99.99m, StockQuantity = 50, CreatedAt = seedDate },
            new Product { Id = 32, Name = "In-Ear Monitors", Category = "Audio", Price = 59.99m, StockQuantity = 90, CreatedAt = seedDate },
            new Product { Id = 33, Name = "Turntable Bluetooth", Category = "Audio", Price = 139.99m, StockQuantity = 20, CreatedAt = seedDate },
            new Product { Id = 34, Name = "DAC/Amp Combo", Category = "Audio", Price = 89.99m, StockQuantity = 40, CreatedAt = seedDate },

            // Office (15)
            new Product { Id = 10, Name = "Ergonomic Chair Cushion", Category = "Office", Price = 44.99m, StockQuantity = 100, CreatedAt = seedDate },
            new Product { Id = 35, Name = "Standing Desk Converter", Category = "Office", Price = 279.99m, StockQuantity = 25, CreatedAt = seedDate },
            new Product { Id = 36, Name = "Whiteboard 36x24", Category = "Office", Price = 39.99m, StockQuantity = 65, CreatedAt = seedDate },
            new Product { Id = 37, Name = "Desk Organizer Set", Category = "Office", Price = 24.99m, StockQuantity = 140, CreatedAt = seedDate },
            new Product { Id = 38, Name = "Filing Cabinet 3-Drawer", Category = "Office", Price = 119.99m, StockQuantity = 30, CreatedAt = seedDate },
            new Product { Id = 39, Name = "Paper Shredder", Category = "Office", Price = 59.99m, StockQuantity = 55, CreatedAt = seedDate },
            new Product { Id = 40, Name = "Label Maker", Category = "Office", Price = 29.99m, StockQuantity = 110, CreatedAt = seedDate },
            new Product { Id = 41, Name = "Desk Calendar 2026", Category = "Office", Price = 12.99m, StockQuantity = 200, CreatedAt = seedDate },
            new Product { Id = 42, Name = "Ergonomic Footrest", Category = "Office", Price = 49.99m, StockQuantity = 75, CreatedAt = seedDate },
            new Product { Id = 43, Name = "Monitor Arm Mount", Category = "Office", Price = 89.99m, StockQuantity = 60, CreatedAt = seedDate },
            new Product { Id = 44, Name = "Noise Machine White", Category = "Office", Price = 34.99m, StockQuantity = 95, CreatedAt = seedDate },
            new Product { Id = 45, Name = "Bookshelf Desktop", Category = "Office", Price = 54.99m, StockQuantity = 45, CreatedAt = seedDate },
            new Product { Id = 46, Name = "Printer Paper 5-Ream", Category = "Office", Price = 27.99m, StockQuantity = 300, CreatedAt = seedDate },
            new Product { Id = 47, Name = "Sticky Notes Bulk Pack", Category = "Office", Price = 8.99m, StockQuantity = 500, CreatedAt = seedDate },
            new Product { Id = 48, Name = "Dry-Erase Markers 12pk", Category = "Office", Price = 11.99m, StockQuantity = 250, CreatedAt = seedDate },

            // Accessories (15)
            new Product { Id = 4, Name = "Monitor Stand", Category = "Accessories", Price = 39.99m, StockQuantity = 120, CreatedAt = seedDate },
            new Product { Id = 5, Name = "Desk Lamp", Category = "Accessories", Price = 24.99m, StockQuantity = 300, CreatedAt = seedDate },
            new Product { Id = 7, Name = "Laptop Sleeve", Category = "Accessories", Price = 19.99m, StockQuantity = 250, CreatedAt = seedDate },
            new Product { Id = 49, Name = "Laptop Backpack", Category = "Accessories", Price = 59.99m, StockQuantity = 100, CreatedAt = seedDate },
            new Product { Id = 50, Name = "Mouse Pad XL", Category = "Accessories", Price = 14.99m, StockQuantity = 300, CreatedAt = seedDate },
            new Product { Id = 51, Name = "Wrist Rest Keyboard", Category = "Accessories", Price = 16.99m, StockQuantity = 200, CreatedAt = seedDate },
            new Product { Id = 52, Name = "Screen Cleaning Kit", Category = "Accessories", Price = 9.99m, StockQuantity = 350, CreatedAt = seedDate },
            new Product { Id = 53, Name = "Phone Stand Adjustable", Category = "Accessories", Price = 12.99m, StockQuantity = 280, CreatedAt = seedDate },
            new Product { Id = 54, Name = "Keyboard Cover", Category = "Accessories", Price = 7.99m, StockQuantity = 400, CreatedAt = seedDate },
            new Product { Id = 55, Name = "Privacy Screen Filter", Category = "Accessories", Price = 34.99m, StockQuantity = 90, CreatedAt = seedDate },
            new Product { Id = 56, Name = "Webcam Cover 6-Pack", Category = "Accessories", Price = 5.99m, StockQuantity = 600, CreatedAt = seedDate },
            new Product { Id = 57, Name = "Cable Clips 20-Pack", Category = "Accessories", Price = 6.99m, StockQuantity = 500, CreatedAt = seedDate },
            new Product { Id = 58, Name = "Laptop Cooling Pad", Category = "Accessories", Price = 29.99m, StockQuantity = 130, CreatedAt = seedDate },
            new Product { Id = 59, Name = "Headphone Stand", Category = "Accessories", Price = 19.99m, StockQuantity = 170, CreatedAt = seedDate },
            new Product { Id = 60, Name = "Travel Tech Pouch", Category = "Accessories", Price = 22.99m, StockQuantity = 150, CreatedAt = seedDate },

            // Fitness & Wellness (10)
            new Product { Id = 61, Name = "Smart Water Bottle", Category = "Fitness & Wellness", Price = 34.99m, StockQuantity = 120, CreatedAt = seedDate },
            new Product { Id = 62, Name = "Yoga Mat Premium", Category = "Fitness & Wellness", Price = 39.99m, StockQuantity = 80, CreatedAt = seedDate },
            new Product { Id = 63, Name = "Resistance Bands Set", Category = "Fitness & Wellness", Price = 19.99m, StockQuantity = 200, CreatedAt = seedDate },
            new Product { Id = 64, Name = "Fitness Tracker Band", Category = "Fitness & Wellness", Price = 49.99m, StockQuantity = 100, CreatedAt = seedDate },
            new Product { Id = 65, Name = "Foam Roller", Category = "Fitness & Wellness", Price = 24.99m, StockQuantity = 150, CreatedAt = seedDate },
            new Product { Id = 66, Name = "Jump Rope Speed", Category = "Fitness & Wellness", Price = 12.99m, StockQuantity = 250, CreatedAt = seedDate },
            new Product { Id = 67, Name = "Massage Gun Mini", Category = "Fitness & Wellness", Price = 89.99m, StockQuantity = 45, CreatedAt = seedDate },
            new Product { Id = 68, Name = "Balance Board", Category = "Fitness & Wellness", Price = 44.99m, StockQuantity = 60, CreatedAt = seedDate },
            new Product { Id = 69, Name = "Posture Corrector", Category = "Fitness & Wellness", Price = 29.99m, StockQuantity = 140, CreatedAt = seedDate },
            new Product { Id = 70, Name = "Blue Light Glasses", Category = "Fitness & Wellness", Price = 24.99m, StockQuantity = 220, CreatedAt = seedDate },

            // Kitchen & Drinkware (10)
            new Product { Id = 71, Name = "Insulated Travel Mug", Category = "Kitchen & Drinkware", Price = 24.99m, StockQuantity = 200, CreatedAt = seedDate },
            new Product { Id = 72, Name = "Pour-Over Coffee Set", Category = "Kitchen & Drinkware", Price = 34.99m, StockQuantity = 80, CreatedAt = seedDate },
            new Product { Id = 73, Name = "Electric Kettle", Category = "Kitchen & Drinkware", Price = 44.99m, StockQuantity = 65, CreatedAt = seedDate },
            new Product { Id = 74, Name = "Desk Snack Container Set", Category = "Kitchen & Drinkware", Price = 16.99m, StockQuantity = 170, CreatedAt = seedDate },
            new Product { Id = 75, Name = "French Press 34oz", Category = "Kitchen & Drinkware", Price = 29.99m, StockQuantity = 90, CreatedAt = seedDate },
            new Product { Id = 76, Name = "Reusable Straw Set", Category = "Kitchen & Drinkware", Price = 8.99m, StockQuantity = 350, CreatedAt = seedDate },
            new Product { Id = 77, Name = "Lunch Box Bento", Category = "Kitchen & Drinkware", Price = 19.99m, StockQuantity = 140, CreatedAt = seedDate },
            new Product { Id = 78, Name = "Cold Brew Maker", Category = "Kitchen & Drinkware", Price = 27.99m, StockQuantity = 70, CreatedAt = seedDate },
            new Product { Id = 79, Name = "Tea Infuser Bottle", Category = "Kitchen & Drinkware", Price = 15.99m, StockQuantity = 180, CreatedAt = seedDate },
            new Product { Id = 80, Name = "Espresso Cups Set of 4", Category = "Kitchen & Drinkware", Price = 22.99m, StockQuantity = 100, CreatedAt = seedDate },

            // Books & Learning (10)
            new Product { Id = 81, Name = "Clean Code Book", Category = "Books & Learning", Price = 39.99m, StockQuantity = 60, CreatedAt = seedDate },
            new Product { Id = 82, Name = "System Design Interview", Category = "Books & Learning", Price = 35.99m, StockQuantity = 50, CreatedAt = seedDate },
            new Product { Id = 83, Name = "Cloud Architecture Patterns", Category = "Books & Learning", Price = 44.99m, StockQuantity = 40, CreatedAt = seedDate },
            new Product { Id = 84, Name = "Data Science Handbook", Category = "Books & Learning", Price = 49.99m, StockQuantity = 35, CreatedAt = seedDate },
            new Product { Id = 85, Name = "DevOps Handbook", Category = "Books & Learning", Price = 34.99m, StockQuantity = 55, CreatedAt = seedDate },
            new Product { Id = 86, Name = "Kubernetes Up & Running", Category = "Books & Learning", Price = 42.99m, StockQuantity = 45, CreatedAt = seedDate },
            new Product { Id = 87, Name = "Learning SQL", Category = "Books & Learning", Price = 29.99m, StockQuantity = 70, CreatedAt = seedDate },
            new Product { Id = 88, Name = "Designing Data Apps", Category = "Books & Learning", Price = 47.99m, StockQuantity = 30, CreatedAt = seedDate },
            new Product { Id = 89, Name = "The Pragmatic Programmer", Category = "Books & Learning", Price = 41.99m, StockQuantity = 65, CreatedAt = seedDate },
            new Product { Id = 90, Name = "Site Reliability Engineering", Category = "Books & Learning", Price = 38.99m, StockQuantity = 50, CreatedAt = seedDate },

            // Gaming (10)
            new Product { Id = 91, Name = "Gaming Mouse RGB", Category = "Gaming", Price = 49.99m, StockQuantity = 100, CreatedAt = seedDate },
            new Product { Id = 92, Name = "Mechanical Gaming Keyboard", Category = "Gaming", Price = 119.99m, StockQuantity = 50, CreatedAt = seedDate },
            new Product { Id = 93, Name = "Controller Wireless", Category = "Gaming", Price = 59.99m, StockQuantity = 80, CreatedAt = seedDate },
            new Product { Id = 94, Name = "Gaming Headset 7.1", Category = "Gaming", Price = 79.99m, StockQuantity = 65, CreatedAt = seedDate },
            new Product { Id = 95, Name = "Mouse Bungee", Category = "Gaming", Price = 14.99m, StockQuantity = 200, CreatedAt = seedDate },
            new Product { Id = 96, Name = "Stream Deck Mini", Category = "Gaming", Price = 79.99m, StockQuantity = 40, CreatedAt = seedDate },
            new Product { Id = 97, Name = "Gaming Desk Mat XXL", Category = "Gaming", Price = 29.99m, StockQuantity = 150, CreatedAt = seedDate },
            new Product { Id = 98, Name = "Capture Card HD", Category = "Gaming", Price = 129.99m, StockQuantity = 30, CreatedAt = seedDate },
            new Product { Id = 99, Name = "VR Headset Stand", Category = "Gaming", Price = 24.99m, StockQuantity = 75, CreatedAt = seedDate },
            new Product { Id = 100, Name = "LED Light Strip RGB 10ft", Category = "Gaming", Price = 18.99m, StockQuantity = 250, CreatedAt = seedDate }
        );
    }
}
