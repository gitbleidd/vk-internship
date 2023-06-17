using System.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VkInternship.App.Models;
using VkInternship.Data;
using VkInternship.Data.Entities;
using OneOf;

namespace VkInternship.App.Services;

public class UserService
{
    public record struct UserNotFound;
    public record struct UserNameTaken;
    public record struct UnknownUserGroup;
    public record struct AdminAlreadyExists;
    public record struct UserDeleted;
    
    private readonly UsersContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;

    public UserService(UsersContext context, ILogger<UserService> logger, IMemoryCache memoryCache, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _memoryCache = memoryCache;
        _mapper = mapper;
    }

    public async Task<OneOf<UserInfo, UserNotFound>> GetUserAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.State)
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == id && u.State.Code == UserState.State.Active);

        if (user is null)
            return new UserNotFound();

        var userInfo = _mapper.Map<UserInfo>(user);
        return userInfo;
    }
    
    
    public async Task<OneOf<List<UserInfo>>> GetUserAsync(int limit, int offset)
    {
        return await _context.Users
            .Include(u => u.State)
            .Include(u => u.Group)
            .Where(u => u.State.Code == UserState.State.Active)
            .OrderBy(u => u.Id)
            .Skip(offset).Take(limit)
            .Select(user => _mapper.Map<UserInfo>(user))
            .ToListAsync();
    }

    public async Task<OneOf<UserInfo, UserNameTaken, UnknownUserGroup, AdminAlreadyExists>> CreateUserAsync(
        UserRegistrationInfo userRegistrationInfo)
    {
        // Чтобы гарантировать уникальность логинов пользователя, мы создаём их в транзакции
        // с уровнем изоляции SERIALIZABLE.
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        
        var user = await _context.Users
            .Include(u => u.State)
            .FirstOrDefaultAsync(u => u.State.Code == UserState.State.Active && u.Login == userRegistrationInfo.Login);

        if (user is not null)
            return new UserNameTaken();

        if (!Enum.TryParse(userRegistrationInfo.Group, true, out UserGroup.Group registrationGroup))
            return new UnknownUserGroup();

        if (registrationGroup == UserGroup.Group.Admin)
        {
            var admin = await _context.Users
                .Include(u => u.Group)
                .Include(u => u.State)
                .FirstOrDefaultAsync(u =>
                    u.State.Code == UserState.State.Active && u.Group.Code == UserGroup.Group.Admin);

            if (admin is not null)
                return new AdminAlreadyExists();
        }

        var states = await GetCachedStatesAsync();
        var activeState = states.First(state => state.Code == UserState.State.Active);

        var groups = await GetCachedGroupsAsync();
        var group = groups.First(u => u.Code == registrationGroup);
        
        var newUser = new User
        {
            Login = userRegistrationInfo.Login,
            Password = BCrypt.Net.BCrypt.HashPassword(userRegistrationInfo.Password),
            StateId = activeState.Id,
            GroupId = group.Id
        };
        _context.Users.Add(newUser);
        
        // Имитация задержки создания пользователя 
        await Task.Delay(TimeSpan.FromSeconds(5));
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new UserInfo
        {
            Id = newUser.Id,
            Login = newUser.Login,
            CreatedDate = newUser.CreatedDate,
            Group = group.Code.ToString(),
            State = activeState.Code.ToString()
        };
    }

    public async Task<OneOf<UserDeleted, UserNotFound>> DeleteUserAsync(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id && u.State.Code == UserState.State.Active);

        if (user is null)
            return new UserNotFound();

        var states = await GetCachedStatesAsync();
        var blockedState = states.First(state => state.Code == UserState.State.Blocked);
        user.State = blockedState;
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return new UserDeleted();
    }

    private async Task<List<UserState>> GetCachedStatesAsync()
    {
        var cachedStates = await _memoryCache.GetOrCreateAsync("_states", async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _context.UserStates.AsNoTracking().ToListAsync();
        });
        return cachedStates ?? new List<UserState>();
    }


    private async Task<List<UserGroup>> GetCachedGroupsAsync()
    {
        var cachedGroups = await _memoryCache.GetOrCreateAsync("_groups", async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _context.UserGroups.AsNoTracking().ToListAsync();
        });
        return cachedGroups ?? new List<UserGroup>();
    }
        
}