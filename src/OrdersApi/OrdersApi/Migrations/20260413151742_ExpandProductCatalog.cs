using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrdersApi.Migrations
{
    /// <inheritdoc />
    public partial class ExpandProductCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Name", "Price", "StockQuantity" },
                values: new object[,]
                {
                    { 11, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "27\" 4K Monitor", 349.99m, 40 },
                    { 12, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Portable SSD 1TB", 89.99m, 120 },
                    { 13, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wireless Charger Pad", 19.99m, 300 },
                    { 14, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HDMI Cable 6ft", 9.99m, 500 },
                    { 15, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "USB Flash Drive 64GB", 12.99m, 400 },
                    { 16, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smart Power Strip", 34.99m, 150 },
                    { 17, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tablet Stand", 24.99m, 200 },
                    { 18, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop Docking Station", 149.99m, 55 },
                    { 19, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ethernet Adapter USB-C", 18.99m, 250 },
                    { 20, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wireless Presenter Remote", 29.99m, 130 },
                    { 21, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Surge Protector 8-Outlet", 22.99m, 180 },
                    { 22, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MicroSD Card 128GB", 17.99m, 350 },
                    { 23, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Portable Monitor 15.6\"", 199.99m, 35 },
                    { 24, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cable Management Kit", 14.99m, 270 },
                    { 25, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smart Plug 4-Pack", 29.99m, 160 },
                    { 26, "Electronics", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "USB Microphone", 69.99m, 85 },
                    { 27, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wireless Earbuds", 79.99m, 110 },
                    { 28, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Studio Monitor Speakers", 249.99m, 25 },
                    { 29, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soundbar 2.1", 159.99m, 45 },
                    { 30, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Podcast Starter Kit", 129.99m, 30 },
                    { 31, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Audio Interface USB", 99.99m, 50 },
                    { 32, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "In-Ear Monitors", 59.99m, 90 },
                    { 33, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Turntable Bluetooth", 139.99m, 20 },
                    { 34, "Audio", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DAC/Amp Combo", 89.99m, 40 },
                    { 35, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Standing Desk Converter", 279.99m, 25 },
                    { 36, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Whiteboard 36x24", 39.99m, 65 },
                    { 37, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desk Organizer Set", 24.99m, 140 },
                    { 38, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Filing Cabinet 3-Drawer", 119.99m, 30 },
                    { 39, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paper Shredder", 59.99m, 55 },
                    { 40, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Label Maker", 29.99m, 110 },
                    { 41, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desk Calendar 2026", 12.99m, 200 },
                    { 42, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic Footrest", 49.99m, 75 },
                    { 43, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Monitor Arm Mount", 89.99m, 60 },
                    { 44, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Noise Machine White", 34.99m, 95 },
                    { 45, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bookshelf Desktop", 54.99m, 45 },
                    { 46, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Printer Paper 5-Ream", 27.99m, 300 },
                    { 47, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sticky Notes Bulk Pack", 8.99m, 500 },
                    { 48, "Office", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dry-Erase Markers 12pk", 11.99m, 250 },
                    { 49, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop Backpack", 59.99m, 100 },
                    { 50, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mouse Pad XL", 14.99m, 300 },
                    { 51, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wrist Rest Keyboard", 16.99m, 200 },
                    { 52, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Screen Cleaning Kit", 9.99m, 350 },
                    { 53, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Phone Stand Adjustable", 12.99m, 280 },
                    { 54, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Keyboard Cover", 7.99m, 400 },
                    { 55, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Privacy Screen Filter", 34.99m, 90 },
                    { 56, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Webcam Cover 6-Pack", 5.99m, 600 },
                    { 57, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cable Clips 20-Pack", 6.99m, 500 },
                    { 58, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Laptop Cooling Pad", 29.99m, 130 },
                    { 59, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Headphone Stand", 19.99m, 170 },
                    { 60, "Accessories", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Travel Tech Pouch", 22.99m, 150 },
                    { 61, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smart Water Bottle", 34.99m, 120 },
                    { 62, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Yoga Mat Premium", 39.99m, 80 },
                    { 63, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Resistance Bands Set", 19.99m, 200 },
                    { 64, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fitness Tracker Band", 49.99m, 100 },
                    { 65, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Foam Roller", 24.99m, 150 },
                    { 66, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jump Rope Speed", 12.99m, 250 },
                    { 67, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Massage Gun Mini", 89.99m, 45 },
                    { 68, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Balance Board", 44.99m, 60 },
                    { 69, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Posture Corrector", 29.99m, 140 },
                    { 70, "Fitness & Wellness", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blue Light Glasses", 24.99m, 220 },
                    { 71, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Insulated Travel Mug", 24.99m, 200 },
                    { 72, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pour-Over Coffee Set", 34.99m, 80 },
                    { 73, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Electric Kettle", 44.99m, 65 },
                    { 74, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Desk Snack Container Set", 16.99m, 170 },
                    { 75, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "French Press 34oz", 29.99m, 90 },
                    { 76, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reusable Straw Set", 8.99m, 350 },
                    { 77, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lunch Box Bento", 19.99m, 140 },
                    { 78, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cold Brew Maker", 27.99m, 70 },
                    { 79, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tea Infuser Bottle", 15.99m, 180 },
                    { 80, "Kitchen & Drinkware", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Espresso Cups Set of 4", 22.99m, 100 },
                    { 81, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Clean Code Book", 39.99m, 60 },
                    { 82, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System Design Interview", 35.99m, 50 },
                    { 83, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cloud Architecture Patterns", 44.99m, 40 },
                    { 84, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Data Science Handbook", 49.99m, 35 },
                    { 85, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DevOps Handbook", 34.99m, 55 },
                    { 86, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kubernetes Up & Running", 42.99m, 45 },
                    { 87, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Learning SQL", 29.99m, 70 },
                    { 88, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Designing Data Apps", 47.99m, 30 },
                    { 89, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The Pragmatic Programmer", 41.99m, 65 },
                    { 90, "Books & Learning", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Site Reliability Engineering", 38.99m, 50 },
                    { 91, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gaming Mouse RGB", 49.99m, 100 },
                    { 92, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mechanical Gaming Keyboard", 119.99m, 50 },
                    { 93, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Controller Wireless", 59.99m, 80 },
                    { 94, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gaming Headset 7.1", 79.99m, 65 },
                    { 95, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mouse Bungee", 14.99m, 200 },
                    { 96, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stream Deck Mini", 79.99m, 40 },
                    { 97, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gaming Desk Mat XXL", 29.99m, 150 },
                    { 98, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Capture Card HD", 129.99m, 30 },
                    { 99, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VR Headset Stand", 24.99m, 75 },
                    { 100, "Gaming", new DateTime(2016, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "LED Light Strip RGB 10ft", 18.99m, 250 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}
