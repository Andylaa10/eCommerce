using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.EntityFrameworkCore.Extensions;
using ProductService.Core.Entities;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace ProductService.Core.Helpers;

public class DatabaseContext : DbContext
{
    private readonly AppSettings.AppSettings _appSettings;

    public DatabaseContext(IOptions<AppSettings.AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMongoDB(_appSettings.ProductDB, "ProductDB");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().ToCollection("Products");
    }

    public DbSet<Product> Products { get; init; }
}