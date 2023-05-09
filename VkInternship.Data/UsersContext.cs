using Microsoft.EntityFrameworkCore;
using VkInternship.Data.Entities;

namespace VkInternship.Data;

public class UsersContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<UserState> UserStates => Set<UserState>();

    public UsersContext(DbContextOptions<UsersContext> options) : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Constants.SchemaName);

        modelBuilder.Entity<UserGroup>().HasData(new List<UserGroup>()
        {
            new() { Id = 1, Code = "Admin" },
            new() { Id = 2, Code = "User" }
        });
        
        modelBuilder.Entity<UserState>().HasData(new List<UserState>()
        {
            new() { Id = 1, Code = "Active" },
            new() { Id = 2, Code = "Blocked" }
        });

        base.OnModelCreating(modelBuilder);
    }
}