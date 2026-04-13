using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrdersApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "City", "CreatedAt", "Email", "FirstName", "LastName", "State" },
                values: new object[,]
                {
                    { 1, "Seattle", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alice@example.com", "Alice", "Johnson", "WA" },
                    { 2, "Portland", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bob@example.com", "Bob", "Smith", "OR" },
                    { 3, "San Francisco", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "carol@example.com", "Carol", "Williams", "CA" },
                    { 4, "Denver", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "david@example.com", "David", "Brown", "CO" },
                    { 5, "Austin", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "eva@example.com", "Eva", "Davis", "TX" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wireless Mouse", 29.99m, 150 },
                    { 2, "Electronics", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mechanical Keyboard", 89.99m, 75 },
                    { 3, "Electronics", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "USB-C Hub", 49.99m, 200 },
                    { 4, "Accessories", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Monitor Stand", 39.99m, 120 },
                    { 5, "Accessories", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desk Lamp", 24.99m, 300 },
                    { 6, "Electronics", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Webcam HD", 59.99m, 90 },
                    { 7, "Accessories", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop Sleeve", 19.99m, 250 },
                    { 8, "Audio", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Noise-Cancelling Headphones", 199.99m, 60 },
                    { 9, "Audio", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bluetooth Speaker", 34.99m, 180 },
                    { 10, "Office", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic Chair Cushion", 44.99m, 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
