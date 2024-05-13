using AuthService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Core.Helpers;

public class DatabaseContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("ConnectionString"); // TODO        
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
    
    public DbSet<Auth> Auths { get; set; }
}