using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SultanSklep.Models;
using System;

namespace SultanSklep.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ProductOperation> ProductOperations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProductOperation -> Address
            modelBuilder.Entity<ProductOperation>()
                .HasOne(po => po.Address)
                .WithMany()
                .HasForeignKey(po => po.AddressID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict istifadə edildi

            // ProductOperation -> User
            modelBuilder.Entity<ProductOperation>()
                .HasOne(po => po.User)
                .WithMany()
                .HasForeignKey(po => po.UserID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict istifadə edildi

            // ProductOperation -> Product
            modelBuilder.Entity<ProductOperation>()
                .HasOne(po => po.Product)
                .WithMany()
                .HasForeignKey(po => po.ProductID)
                .OnDelete(DeleteBehavior.Restrict); // Restrict istifadə edildi

            // Product üçün dəqiqlik təyin etmək
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)"); // Məbləğ üçün dəqiqlik
        }
    }
}