using Microsoft.EntityFrameworkCore;
using UserService.Core.Entities;

namespace UserService.Core.Helpers;

public class DatabaseContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("ConnectionString"); // TODO        
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
    
    public DbSet<User> Users { get; set; }
}