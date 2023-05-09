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
        
        modelBuilder
            .Entity<UserGroup>()
            .Property(e => e.Code)
            .HasConversion(
                v => v.ToString(),
                v => (UserGroup.Group)Enum.Parse(typeof(UserGroup.Group), v));

        modelBuilder
            .Entity<UserState>()
            .Property(e => e.Code)
            .HasConversion(
                v => v.ToString(),
                v => (UserState.State)Enum.Parse(typeof(UserState.State), v));

        modelBuilder.Entity<UserGroup>().HasData(new List<UserGroup>()
        {
            new() { Id = 1, Code = UserGroup.Group.Admin },
            new() { Id = 2, Code = UserGroup.Group.User }
        });
        
        modelBuilder.Entity<UserState>().HasData(new List<UserState>()
        {
            new() { Id = 1, Code = UserState.State.Active },
            new() { Id = 2, Code = UserState.State.Blocked }
        });

        base.OnModelCreating(modelBuilder);
    }
}