using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace UserService.Core.Helpers;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigurationManager.AppSettings.Get("USER_SERVICE_CONNECTION_STRING")!;

        Console.WriteLine(ConfigurationManager.AppSettings.Keys.Count);
        //optionsBuilder.UseNpgsql(connectionString); // TODO        
        optionsBuilder.UseNpgsql("Server=userdb;Port=5432;Database=UserDB;Username=postgres;Password=postgres");      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Setup DB

        //Auto generate id
        modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();

        //Constraints 
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        #endregion
    }

    public DbSet<User> Users { get; init; }
}