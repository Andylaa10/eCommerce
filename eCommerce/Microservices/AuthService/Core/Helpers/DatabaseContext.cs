using AuthService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ConfigurationManager = System.Configuration.ConfigurationManager;


namespace AuthService.Core.Helpers;

public class DatabaseContext : DbContext
{
    private readonly AppSettings.AppSettings _appSettings;
    public DatabaseContext(IOptions<AppSettings.AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        // Postgres doesnt support c#'s DateTime, thats why we need this
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_appSettings.AuthDB);
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