using Microsoft.EntityFrameworkCore;
using StoreManagement.Models;
using System.Reflection.Metadata;
using static Azure.Core.HttpHeader;

namespace StoreManagement.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Items> Items { get; set; }
        public DbSet<ItemDetails> ItemDetails { get; set; }
        public DbSet<ItemMenu> ItemMenus { get; set; }
        public DbSet<ItemMenuDetail> ItemMenuDtls { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Combos> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<UserInfoMation> UserInfoMations { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>().HasKey(e=>e.CategoryId);
            modelBuilder.Entity<Items>().HasKey(e => e.ItemId);
            modelBuilder.Entity<Items>()
            .HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CategoryId);

            modelBuilder.Entity<Customers>().HasKey(e => e.CustomerId);
            modelBuilder.Entity<Guest>().HasKey(e => e.GuestId);
            modelBuilder.Entity<Carts>().HasKey(e => e.CartId);
            modelBuilder.Entity<CartDetails>().HasKey(e => e.CartDtlId);
            //modelBuilder.Entity<Carts>()
            //.HasOne<Customers>()
            //.WithMany()
            //.HasForeignKey(c => c.CustomerId);
            modelBuilder.Entity<CartDetails>()
            .HasOne<Carts>()
            .WithMany()
            .HasForeignKey(cd => cd.CartId);
            modelBuilder.Entity<CartDetails>()
            .HasOne<Items>()
            .WithMany()
            .HasForeignKey(cd => cd.ItemId);
            //modelBuilder.Entity<Carts>()
            // .HasOne<Guest>()
            // .WithMany();

            modelBuilder.Entity<ItemDetails>().HasKey(e => e.ItemDtId);
            modelBuilder.Entity<ItemMenu>().HasKey(e=>e.Id);
            modelBuilder.Entity<ItemMenuDetail>().HasKey(e => e.Id);
            modelBuilder.Entity<ItemImage>().HasKey(e => e.ImageId);
            modelBuilder.Entity<ItemImage>()
            .HasOne(ii => ii.Item)
            .WithMany(i => i.ItemImages);

            modelBuilder.Entity<ItemMenuDetail>()
            .HasOne<ItemMenu>()
            .WithMany();

            modelBuilder.Entity<ItemMenuDetail>()
            .HasOne<Items>()
            .WithMany(i => i.ItemMenuDetails);

            modelBuilder.Entity<ItemDetails>()
            .HasOne(id => id.Item)
            .WithMany(i => i.ItemDetails);

            modelBuilder.Entity<Combos>().HasKey(e => e.ComboId);
            modelBuilder.Entity<ComboItem>()
            .HasKey(ci =>ci.ComboDtlId);
            modelBuilder.Entity<ComboItem>()
            .HasOne(ci => ci.Combo)
            .WithMany(c => c.ComboItems);

            modelBuilder.Entity<ComboItem>()
                .HasOne(ci => ci.Item)
                .WithMany(i => i.ComboItems);

            modelBuilder.Entity<Users>().HasKey(e => e.UserId);
            modelBuilder.Entity<UserInfoMation>().HasKey(e => e.UserId);
            modelBuilder.Entity<Orders>().HasKey(e => e.OrderId);
            modelBuilder.Entity<OrderDetails>().HasKey(e => e.OrderDtlId);
            modelBuilder.Entity<Orders>()
            .HasOne(o => o.Guest)
            .WithMany(g => g.Orders);

            //modelBuilder.Entity<Orders>()
            //.HasOne(o => o.Customers)
            //.WithMany(g => g.Orders);

            modelBuilder.Entity<OrderDetails>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails);

            modelBuilder.Entity<OrderDetails>()
            .HasOne(od => od.ItemMenu)
            .WithMany(im => im.OrderDetails);

        }
    }
}
