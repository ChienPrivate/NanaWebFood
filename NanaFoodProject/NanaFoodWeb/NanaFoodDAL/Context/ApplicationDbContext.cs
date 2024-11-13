using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanaFoodDAL.Model;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NanaFoodApi")]
namespace NanaFoodDAL.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<ProductChangeLog> ProductChangeLogs { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            /*List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name = "admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole()
                {
                    Name = "customer",
                    NormalizedName = "CUSTOMER",
                }
                new IdentityRole()
                {
                    Name = "employee",
                    NormalizedName = "EMPLOYEE"
                }
            };*/
            /*builder.Entity<IdentityRole>().HasData(roles);*/
            // Cấu hình cho khóa chính
            builder.Entity<Coupon>().ToTable("Coupon");
            builder.Entity<UserCoupon>().HasKey(e => e.Id);

            builder.Entity<Category>().HasKey(e => e.CategoryId);

            builder.Entity<Product>().HasKey(e => e.ProductId);

            builder.Entity<ProductChangeLog>().HasKey(e => e.LogId);

            builder.Entity<Review>().HasKey(e => e.ReviewId);

            builder.Entity<Order>().HasKey(e => e.OrderId);


            // Cấu hình khóa chính và khóa ngoại cho WishList
            builder.Entity<WishList>().HasKey(e => new { e.ProductId, e.UserId });

            builder.Entity<WishList>()
                .HasOne(e => e.User)
                .WithMany(e => e.WishLists)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WishList>()
                .HasOne(e => e.Product)
                .WithMany(e => e.WishLists)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình khóa chính và khóa ngoại cho CartDetails
            builder.Entity<CartDetails>().HasKey(e => new { e.ProductId, e.UserId });

            builder.Entity<CartDetails>()
                .HasOne(e => e.User)
                .WithMany(e => e.CartDetails)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartDetails>()
                .HasOne(e => e.Product)
                .WithMany(e => e.CartDetails)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình khóa chính và khóa ngoại cho OrderDetails
            builder.Entity<OrderDetails>().HasKey(e => new { e.ProductId, e.OrderId });

            builder.Entity<OrderDetails>()
                .HasOne(e => e.Order)
                .WithMany(e => e.OrderDetails)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderDetails>()
                .HasOne(e => e.Product)
                .WithMany(e => e.OrderDetails)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserCoupon>()
                .HasOne(e => e.Coupon)
                .WithMany(e=>e.UserCoupons)
                .HasForeignKey(e=>e.CouponCode);
        }

    }
}
