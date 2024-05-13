using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ProductService.Core.Entities;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace ProductService.Core.Helpers;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigurationManager.AppSettings.Get("PRODUCT_SERVICE_CONNECTION_STRING");

        optionsBuilder.UseMongoDB(connectionString, "ProductDB");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().ToCollection("products");
    }

    public DbSet<Product> Products { get; init; }
}