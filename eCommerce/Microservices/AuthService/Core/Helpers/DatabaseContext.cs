using AuthService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using ConfigurationManager = System.Configuration.ConfigurationManager;


namespace AuthService.Core.Helpers;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigurationManager.AppSettings.Get("AUTH_SERVICE_CONNECTION_STRING");

        //optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseNpgsql("Server=authdb;Port=5433;Database=AuthDB;Username=postgres;Password=postgres");      

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Setup DB

        //Auto generate id
        modelBuilder.Entity<Auth>().Property(a => a.Id).ValueGeneratedOnAdd();

        //Constraints 
        modelBuilder.Entity<Auth>().HasIndex(a => a.Email).IsUnique();

        #endregion
    }

    public DbSet<Auth> Auths { get; init; }
}