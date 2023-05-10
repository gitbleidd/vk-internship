using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using VkInternship.App.Mappings;
using VkInternship.App.Models;
using VkInternship.App.Services;
using VkInternship.Data;
using VkInternship.Data.Entities;

namespace VkInternship.Tests;

public class UserServiceTests
{
    private readonly DbContextOptions<UsersContext> _dbContextOptions;
    private readonly IMapper _mapper;

    public UserServiceTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<UsersContext>()
            .UseInMemoryDatabase(databaseName: "VkInternshipDb")
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        var mapperConfiguration = new MapperConfiguration(config =>
        {
            config.AddProfile(new MappingProfiles());
        });
        _mapper = mapperConfiguration.CreateMapper();
    }

    private async Task<UsersContext> GetInMemoryUsersContext()
    {
        var context = new UsersContext(_dbContextOptions);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        return context;
    }

    [Fact]
    public async Task CreateUser_UserAlreadyExists()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);
        
        var group = await context.UserGroups.FirstAsync(g => g.Code == UserGroup.Group.User);
        var state = await context.UserStates.FirstAsync(g => g.Code == UserState.State.Active);
        var user = new User
        {
            Login = "foo",
            Password = "1234",
            Group = group,
            State = state
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        // Act
        var createUser2Result = await userService.CreateUserAsync(
            new UserRegistrationInfo("foo", "1234", UserGroup.Group.User.ToString())
        );
        
        // Assert
        Assert.Equal(new UserService.UserNameTaken(), createUser2Result.Value);
    }

    [Fact]
    public async Task CreateUser_UnknownUserGroup()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);
        
        // Act
        var createUserResult =
            await userService.CreateUserAsync(new UserRegistrationInfo("foo", "1234", "unknown group"));

        // Assert
        Assert.Equal(new UserService.UnknownUserGroup(), createUserResult.Value);
    }
    
    [Fact]
    public async Task CreateUser_AdminAlreadyExists()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);
        
        var group = await context.UserGroups.FirstAsync(g => g.Code == UserGroup.Group.Admin);
        var state = await context.UserStates.FirstAsync(g => g.Code == UserState.State.Active);
        var user = new User
        {
            Login = "foo",
            Password = "1234",
            Group = group,
            State = state
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        // Act
        var createUser2Result = await userService.CreateUserAsync(
            new UserRegistrationInfo("bar", "0987", UserGroup.Group.Admin.ToString())
        );

        // Assert
        Assert.Equal(new UserService.AdminAlreadyExists(), createUser2Result.Value);
    }

    [Fact]
    public async Task DeleteUser_UserSuccessfullyDeleted()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);

        var group = await context.UserGroups.FirstAsync(g => g.Code == UserGroup.Group.User);
        var state = await context.UserStates.FirstAsync(g => g.Code == UserState.State.Active);
        var user = new User
        {
            Login = "foo",
            Password = "1234",
            Group = group,
            State = state
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Act
        var deleteUserAsync = await userService.DeleteUserAsync(user.Id);

        // Assert
        Assert.Equal(new UserService.UserDeleted(), deleteUserAsync.Value);
    }
    
    [Fact]
    public async Task DeleteUser_UserNotFound()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);

        // Act
        var deleteUserAsync = await userService.DeleteUserAsync(int.MaxValue);

        // Assert
        Assert.Equal(new UserService.UserNotFound(), deleteUserAsync.Value);
    }
    
    [Fact]
    public async Task GetUser_UserSuccessfullyObtained()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);
        
        var group = await context.UserGroups.FirstAsync(g => g.Code == UserGroup.Group.User);
        var state = await context.UserStates.FirstAsync(g => g.Code == UserState.State.Active);
        var user = new User
        {
            Login = "foo",
            Password = "1234",
            Group = group,
            State = state
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        
        // Act
        var getUserAsync = await userService.GetUserAsync(user.Id);

        // Assert
        var expectedUser = new UserInfo()
        {
            Id = user.Id,
            Login = user.Login,
            CreatedDate = user.CreatedDate,
            Group = user.Group.Code.ToString()
        };
        Assert.Equal(expectedUser, getUserAsync.Value);
    }

    [Fact]
    public async Task GetUser_UserNotFound()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);

        // Act
        var getUserAsync = await userService.GetUserAsync(int.MaxValue);

        // Assert
        Assert.Equal(new UserService.UserNotFound(), getUserAsync.Value);
    }
    
    [Fact]
    public async Task GetUser_Pagination()
    {
        // Arrange
        var context = await GetInMemoryUsersContext();
        var logger = new FakeLogger<UserService>();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var userService = new UserService(context, logger, memoryCache, _mapper);
        
        var group = await context.UserGroups.FirstAsync(g => g.Code == UserGroup.Group.User);
        var state = await context.UserStates.FirstAsync(g => g.Code == UserState.State.Active);
        var user1 = new User { Login = "foo1", Password = "1234", Group = group, State = state };
        var user2 = new User { Login = "foo2", Password = "wasd", Group = group, State = state };
        var user3 = new User { Login = "foo3", Password = "qwerty", Group = group, State = state };
        context.Users.AddRange(user1, user2, user3);
        await context.SaveChangesAsync();

        // Порядок важен
        var expectedUsers = new List<UserInfo>
        {
            new UserInfo
            {
                Id = user1.Id, Login = user1.Login, CreatedDate = user1.CreatedDate, Group = user1.Group.Code.ToString()
            },
            new UserInfo
            {
                Id = user2.Id, Login = user2.Login, CreatedDate = user2.CreatedDate, Group = user2.Group.Code.ToString()
            },
            new UserInfo
            {
                Id = user3.Id, Login = user3.Login, CreatedDate = user3.CreatedDate, Group = user3.Group.Code.ToString()
            },
        };

        // Act
        var getUserAsync = await userService.GetUserAsync(3, 0); // Пользователи возвращаются в порядке возрастания id

        // Assert
        Assert.Equal(expectedUsers, getUserAsync.Value);
    }
}