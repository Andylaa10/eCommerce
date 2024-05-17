using AuthService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using ConfigurationManager = System.Configuration.ConfigurationManager;


namespace AuthService.Core.Helpers;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
        // Postgres doesnt support c#'s DateTime, thats why we need this
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigurationManager.AppSettings.Get("AUTH_SERVICE_CONNECTION_STRING");
        //optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseNpgsql("Host=authdb;Port=5432;Database=AuthDB;Username=postgres;Password=postgres");
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