using Microsoft.EntityFrameworkCore;
using PointOfSales.Entities;
using System;

namespace PointOfSales
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
            // Configure Users entity
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(u => u.Email);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Password).IsRequired().HasMaxLength(100);
                entity.Property(u => u.UserRole).IsRequired().HasMaxLength(50);
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
                entity.HasKey(pi => new { pi.ProductId, pi.Quantity });

                // Configure foreign key relationship
                entity.HasOne<Product>().WithMany().HasForeignKey(pi => pi.ProductId).OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SaleItem entity
            modelBuilder.Entity<SaleItem>(entity =>
            {
                entity.HasKey(si => new { si.ProductId, si.Quantity });

                // Configure foreign key relationship
                entity.HasOne<Product>()
                      .WithMany()
                      .HasForeignKey(si => si.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
