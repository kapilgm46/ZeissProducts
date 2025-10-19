using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeiss.Product.DAL.Models;

namespace Zeiss.Product.DAL.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductIdTracker> ProductIdTrackers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductModel>().HasIndex(p=>p.ProductId).IsUnique();
            modelBuilder.Entity<ProductModel>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<ProductIdTracker>().HasData(new ProductIdTracker { Id = 1, LastId = 99999 });
        }
    }
}
