using Microsoft.EntityFrameworkCore;
using PetShop.Repositories.Models;

namespace PetShop.Repositories.DBContext
{
    public class PetShopDbContext : DbContext
    {
        public PetShopDbContext() { }

        public PetShopDbContext(DbContextOptions<PetShopDbContext> options)
            : base(options) { }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===================== USER =====================
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // ===================== USER ADDRESS =====================
            modelBuilder.Entity<UserAddress>()
                .HasKey(a => a.AddressId);

            modelBuilder.Entity<UserAddress>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // OK: User xoá → Address xoá

            // ===================== CATEGORY - PRODUCT =====================
            modelBuilder.Entity<Product>()
                .HasOne<Category>()
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================== CART - CART ITEM =====================
            modelBuilder.Entity<CartItem>()
                .HasOne<Cart>()
                .WithMany()
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Restrict); // ⚠️ Đổi thành Restrict

            modelBuilder.Entity<CartItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================== ORDER - ORDER DETAIL =====================
            modelBuilder.Entity<OrderDetail>()
                .HasOne<Order>()
                .WithMany()
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict); // ⚠️ đổi Cascade -> Restrict

            modelBuilder.Entity<OrderDetail>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================== PAYMENT - ORDER =====================
            modelBuilder.Entity<Payment>()
                .HasOne<Order>()
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================== CHAT ROOM - USER =====================
            modelBuilder.Entity<ChatRoom>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatRoom>()
                .HasOne(c => c.Admin)
                .WithMany()
                .HasForeignKey(c => c.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================== MESSAGE - CHAT ROOM =====================
            modelBuilder.Entity<Message>()
                .HasOne(m => m.ChatRoom)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===================== NOTIFICATION - USER =====================
            modelBuilder.Entity<Notification>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
