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

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Id, u.StateId });
        
        modelBuilder.Entity<UserGroup>()
            .HasIndex(g => g.Code)
            .IsUnique();
        
        modelBuilder.Entity<UserState>()
            .HasIndex(g => g.Code)
            .IsUnique();
        
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
            new() { Id = (int)UserGroup.Group.Admin, Code = UserGroup.Group.Admin },
            new() { Id = (int)UserGroup.Group.User, Code = UserGroup.Group.User }
        });
        
        modelBuilder.Entity<UserState>().HasData(new List<UserState>()
        {
            new() { Id = (int)UserState.State.Active, Code = UserState.State.Active },
            new() { Id = (int)UserState.State.Blocked, Code = UserState.State.Blocked }
        });

        base.OnModelCreating(modelBuilder);
    }
}