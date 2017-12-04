using Microsoft.EntityFrameworkCore;
using MyStore.BL;
using MyStore.BL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;

namespace MyStore.Data
{
    public class StoreContext : DbContext
    {
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        static StoreContext()
        {
            using (var ctx = new StoreContext())
            {
                ctx.Database.GetDbConnection().StateChange += (s, e) =>
                {
                    if (e.CurrentState == ConnectionState.Open)
                        ctx.Database.ExecuteSqlCommand("PRAGMA foreign_keys = true;");
                };
                ctx.Database.Migrate();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=MyStore.db");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .HasKey(i => new { i.OrderId, i.ProductId });

            modelBuilder.Entity<ShoppingCartItem>()
                .HasKey(i => new { i.ShoppingCartId, i.ProductId });

            modelBuilder.Entity<Product>()
                .Property(p => p.ListPrice)
                .HasColumnType("money(18,0)");

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.ListPrice)
                .HasColumnType("money(18,0)");
        }

        public override int SaveChanges()
        {
            var validationErrors = ChangeTracker
                .Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(r => r != ValidationResult.Success);

            if (validationErrors.Any())
            {
                throw new ValidationException(validationErrors
                    .Select(r => r.ErrorMessage)
                    .Aggregate((err1, err2) => $"{err1}\n{err2}"));
            }

            return base.SaveChanges();
        }
    }
}
