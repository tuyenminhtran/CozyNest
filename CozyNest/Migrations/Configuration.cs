namespace CozyNest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using CozyNest.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<CozyNest.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CozyNest.Models.ApplicationDbContext context)
        {
            // ==================== CATEGORIES ====================
            if (!context.Categories.Any())
            {
                context.Categories.AddOrUpdate(c => c.Name,
                    new Category { Name = "Furniture" },
                    new Category { Name = "Lighting" },
                    new Category { Name = "Textiles" },
                    new Category { Name = "Plants" },
                    new Category { Name = "Seating" }
                );
                context.SaveChanges();
            }

            // ==================== PRODUCTS ====================
            if (!context.Products.Any())
            {
                var furniture = context.Categories.First(c => c.Name == "Furniture");
                var lighting = context.Categories.First(c => c.Name == "Lighting");
                var textiles = context.Categories.First(c => c.Name == "Textiles");
                var plants = context.Categories.First(c => c.Name == "Plants");
                var seating = context.Categories.First(c => c.Name == "Seating");

                context.Products.AddOrUpdate(p => p.Name,
                    new Product
                    {
                        Name = "Nordic Lounge Chair",
                        Description = "Bouclé fabric and bent ash wood construction. Designed for intentional living.",
                        Price = 895,
                        ImageUrl = "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?w=800&q=100",
                        Material = "Bouclé fabric, Ash wood",
                        Stock = 24,
                        CategoryId = seating.CategoryId
                    },
                    new Product
                    {
                        Name = "Knot Coffee Table",
                        Description = "Solid walnut with soft, organic curves. A statement piece for any living space.",
                        Price = 650,
                        ImageUrl = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=800&q=100",
                        Material = "Solid Walnut",
                        Stock = 12,
                        CategoryId = furniture.CategoryId
                    },
                    new Product
                    {
                        Name = "Oak Side Table",
                        Description = "Minimalist oak side table with clean lines and warm finish.",
                        Price = 350,
                        ImageUrl = "https://images.unsplash.com/photo-1533090161767-e6ffed986c88?w=800&q=100",
                        Material = "Oak Wood",
                        Stock = 18,
                        CategoryId = furniture.CategoryId
                    },
                    new Product
                    {
                        Name = "Brass Table Lamp",
                        Description = "Brass stem with linen shade, ambient warm glow perfect for any corner.",
                        Price = 120,
                        ImageUrl = "https://images.unsplash.com/photo-1513506003901-1e6a35f78940?w=800&q=100",
                        Material = "Brass, Linen",
                        Stock = 35,
                        CategoryId = lighting.CategoryId
                    },
                    new Product
                    {
                        Name = "Lumina Floor Lamp",
                        Description = "Tall brass floor lamp with adjustable linen shade.",
                        Price = 345,
                        ImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?w=800&q=100",
                        Material = "Brass, Linen",
                        Stock = 15,
                        CategoryId = lighting.CategoryId
                    },
                    new Product
                    {
                        Name = "Ceramic Table Lamp",
                        Description = "Hand-thrown ceramic base with matte cream finish and cotton shade.",
                        Price = 125,
                        ImageUrl = "https://images.unsplash.com/photo-1524484485831-a92ffc0de03f?w=800&q=100",
                        Material = "Ceramic, Cotton",
                        Stock = 3,
                        CategoryId = lighting.CategoryId
                    },
                    new Product
                    {
                        Name = "Linen Throw Pillow",
                        Description = "Soft linen throw pillow with natural texture, perfect for layering.",
                        Price = 45,
                        ImageUrl = "https://images.unsplash.com/photo-1584100936595-c0654b55a2e2?w=800&q=100",
                        Material = "100% Linen",
                        Stock = 60,
                        CategoryId = textiles.CategoryId
                    },
                    new Product
                    {
                        Name = "Woven Throw Blanket",
                        Description = "Hand-woven cotton throw with subtle pattern, warm and lightweight.",
                        Price = 85,
                        ImageUrl = "https://images.unsplash.com/photo-1584100936595-c0654b55a2e2?w=800&q=100",
                        Material = "Cotton",
                        Stock = 50,
                        CategoryId = textiles.CategoryId
                    },
                    new Product
                    {
                        Name = "Terra Ceramic Vase",
                        Description = "Hand-thrown sculptural vessel with matte finish, perfect for dried florals.",
                        Price = 120,
                        ImageUrl = "https://images.unsplash.com/photo-1578500494198-246f612d3b3d?w=800&q=100",
                        Material = "Ceramic",
                        Stock = 40,
                        CategoryId = plants.CategoryId
                    },
                    new Product
                    {
                        Name = "Artisan Ceramic Vase",
                        Description = "Organic shaped vase with earthy tones, handcrafted by local artisans.",
                        Price = 85,
                        ImageUrl = "https://images.unsplash.com/photo-1516455590571-18256e5bb9ff?w=800&q=100",
                        Material = "Stoneware",
                        Stock = 28,
                        CategoryId = plants.CategoryId
                    }
                );
                context.SaveChanges();
            }

            // ==================== ADMIN USER ====================
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                context.Users.AddOrUpdate(u => u.Email,
                    new User
                    {
                        FullName = "Admin CozyNest",
                        Email = "admin@cozynest.com",
                        PasswordHash = HashPassword("admin123"),
                        Role = "Admin",
                        CreatedAt = DateTime.Now
                    }
                );
                context.SaveChanges();
            }

            // ==================== SAMPLE CUSTOMER ====================
            if (!context.Users.Any(u => u.Role == "Customer"))
            {
                var customer = new User
                {
                    FullName = "Jane Doe",
                    Email = "jane@example.com",
                    PasswordHash = HashPassword("123456"),
                    Role = "Customer",
                    CreatedAt = DateTime.Now
                };
                context.Users.AddOrUpdate(u => u.Email, customer);
                context.SaveChanges();

                context.Carts.AddOrUpdate(c => c.UserId,
                    new Cart { UserId = customer.UserId }
                );
                context.SaveChanges();
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}