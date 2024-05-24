using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using CartService.Core.Entities;
using Microsoft.Extensions.Options;


namespace CartService.Core.Helpers;

public class DatabaseContext : DbContext
{
    private readonly AppSettings.AppSettings _appSettings;

    public DatabaseContext(IOptions<AppSettings.AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMongoDB(_appSettings.CartDB, "CartDB");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cart>().ToCollection("Carts");
    }
    public DbSet<Cart> Carts { get; init; }
}