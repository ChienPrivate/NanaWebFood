﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NanaFoodWeb.Models;

namespace NanaFoodWeb.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<CategoryDto> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductChangeLog> ProductChangeLogs { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<SearchHistory> SearchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình cho khóa chính

            builder.Entity<CategoryDto>().HasKey(e => e.CategoryId);

            builder.Entity<Product>().HasKey(e => e.ProductId);

            builder.Entity<ProductChangeLog>().HasKey(e => e.LogId);

            builder.Entity<Cart>().HasKey(e => e.CartId);

            builder.Entity<Review>().HasKey(e => e.ReviewId);

            builder.Entity<Order>().HasKey(e => e.OrderId);

            // Cấu hình khóa ngoại cho bảng Cart
            builder.Entity<Cart>()
                .HasOne(e => e.User)
                .WithOne(e => e.Cart)
                .HasForeignKey<Cart>(e => e.UserId);


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
            builder.Entity<CartDetails>().HasKey(e => new { e.ProductId, e.CartId });

            builder.Entity<CartDetails>()
                .HasOne(e => e.Cart)
                .WithMany(e => e.CartDetails)
                .HasForeignKey(e => e.CartId)
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
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
