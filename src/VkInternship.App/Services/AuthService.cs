using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VkInternship.App.Models;
using VkInternship.Data;
using VkInternship.Data.Entities;
using OneOf;

namespace VkInternship.App.Services;

public class AuthService
{
    private readonly UsersContext _context;
    private readonly ILogger<UserService> _logger;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;

    public AuthService(UsersContext context, ILogger<UserService> logger, IMemoryCache memoryCache, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _memoryCache = memoryCache;
        _mapper = mapper;
    }
    
    public async Task<OneOf<UserInfo, OneOf.Types.Error>> AuthenticateAsync(string login, string password)
    {
        var user = await _context.Users
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Login == login && u.State.Code == UserState.State.Active);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            return new OneOf.Types.Error();

        var userInfo = _mapper.Map<UserInfo>(user);
        return userInfo;
    }
}