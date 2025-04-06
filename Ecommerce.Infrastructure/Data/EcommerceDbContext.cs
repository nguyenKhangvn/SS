using Ecommerce.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Data
{
        public class EcommerceDbContext : DbContext
        {
            public DbSet<User> Users { get; set; }
            public DbSet<Address> Addresses { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Manufacturer> Manufacturers { get; set; }
            public DbSet<Image> Images { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<Discount> Discounts { get; set; }
            public DbSet<Post> Posts { get; set; }
            public DbSet<ProductPost> ProductPosts { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderItem> OrderItems { get; set; }
            public DbSet<Payment> Payments { get; set; }
            public DbSet<Coupon> Coupons { get; set; }
            public DbSet<Shipment> Shipments { get; set; }
            public DbSet<Chat> Chats { get; set; }
            public DbSet<ChatParticipant> ChatParticipants { get; set; }
            public DbSet<Message> Messages { get; set; }
            public DbSet<StoreLocation> StoreLocations { get; set; }
            public DbSet<ProductStoreInventory> ProductStoreInventories { get; set; }

            public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
            {

            }

            //protected override void OnModelCreating(ModelBuilder modelBuilder)
            //{
            //    base.OnModelCreating(modelBuilder);

            //    // --- Cấu hình Fluent API ---
            //    // Ví dụ: Khóa chính phức hợp cho bảng trung gian (nếu không dùng BaseEntity)
            //    modelBuilder.Entity<ProductPost>()
            //        .HasKey(ppp => new { ppp.ProductId, ppp.PostId });
            //    modelBuilder.Entity<ChatParticipant>()
            //        .HasKey(cp => new { cp.ChatId, cp.UserId }); // Nếu không kế thừa BaseEntity
            //    modelBuilder.Entity<ProductStoreInventory>()
            //         .HasKey(psi => new { psi.ProductId, psi.StoreLocationId }); // Nếu không kế thừa BaseEntity

            //    // Ví dụ: Cấu hình quan hệ Many-to-Many rõ ràng
            //    modelBuilder.Entity<ProductPost>()
            //        .HasOne(ppp => ppp.Product)
            //        .WithMany(p => p.ProductPosts)
            //        .HasForeignKey(ppp => ppp.ProductId);
            //    modelBuilder.Entity<ProductPost>()
            //       .HasOne(ppp => ppp.Post)
            //       .WithMany(pp => pp.ProductPosts)
            //       .HasForeignKey(ppp => ppp.PostId);

            //    // Ví dụ: Cấu hình Decimal Precision
            //    modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            //    modelBuilder.Entity<OrderItem>().Property(oi => oi.PriceAtOrder).HasPrecision(18, 2);
            //    // ... thêm các cấu hình khác cho index, constraints, relationships ...

            //    // Tự động cập nhật CreatedAt, UpdatedAt (cần code thêm trong SaveChangesAsync)
            //    // Hoặc cấu hình giá trị mặc định nếu CSDL hỗ trợ
            //    modelBuilder.Entity<BaseEntity>()
            //        .Property(b => b.CreatedAt)
            //        .HasDefaultValueSql("GETUTCDATE()"); // Ví dụ cho SQL Server
            //    modelBuilder.Entity<BaseEntity>()
            //         .Property(b => b.UpdatedAt)
            //         .HasDefaultValueSql("GETUTCDATE()");


            //    // Cấu hình các mối quan hệ khác nếu cần
            //    // Ví dụ: Quan hệ one-to-one giữa Order và Payment
            //    modelBuilder.Entity<Order>()
            //        .HasOne(o => o.Payment)
            //        .WithOne(p => p.Order)
            //        .HasForeignKey<Payment>(p => p.OrderId); // Chỉ định khóa ngoại trên Payment
            //}
        }
}
