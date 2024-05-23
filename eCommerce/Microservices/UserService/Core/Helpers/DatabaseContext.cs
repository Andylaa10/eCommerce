using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserService.Core.Entities;

namespace UserService.Core.Helpers;

public class DatabaseContext : DbContext
{
    private AppSettings.AppSettings _appSettings;
    
    public DatabaseContext(IOptions<AppSettings.AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        // Postgres doesnt support c#'s DateTime, thats why we need this
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_appSettings.UserDB);      
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