using CartService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ConfigurationManager = System.Configuration.ConfigurationManager;


namespace CartService.Core.Helpers;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigurationManager.AppSettings.Get("CART_SERVICE_CONNECTION_STRING");
        
        optionsBuilder.UseMongoDB(connectionString, "CartDB"); //TODO
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cart>().ToCollection("products");
    }

    public DbSet<Cart> Carts { get; init; }
}