using Microsoft.EntityFrameworkCore;
using PointOfSales.Entities;
using System;

namespace PointOfSales.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Users> Users { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Users entity with TPH
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.Email);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(100);
                entity.Property(u => u.UserRole).IsRequired().HasMaxLength(50);

                entity.HasDiscriminator<string>("UserType")
                      .HasValue<Admin>("Admin")
                      .HasValue<Cashier>("Cashier");
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.Property(p => p.Quantity).IsRequired();
                entity.Property(p => p.Type).HasMaxLength(50);
                entity.Property(p => p.Category).HasMaxLength(50);
            });

            // Configure PurchaseItem entity
            modelBuilder.Entity<PurchaseItem>(entity =>
            {
                entity.HasKey(pi => pi.Id);
                entity.Property(pi => pi.Price).HasPrecision(18, 2);
                entity.Property(pi => pi.PurchaseItemName).IsRequired().HasMaxLength(100);
                entity.Property(pi => pi.Quantity).IsRequired();

                // Configure foreign key relationship
                entity.HasOne(pi => pi.Product)
                      .WithMany()
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SaleItem entity
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(si => si.Id);
                entity.Property(si => si.Price).HasPrecision(18, 2);
                entity.Property(si => si.SalesItemName).IsRequired().HasMaxLength(100);
                entity.Property(si => si.Quantity).IsRequired();

                // Configure foreign key relationship
                entity.HasOne(si => si.Product)
                      .WithMany()
                      .HasForeignKey(si => si.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
