using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VkInternship.App.Models;
using VkInternship.Data;
using VkInternship.Data.Entities;

namespace VkInternship.App.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly UsersContext _context;
    private readonly IMapper _mapper;

    public UsersController(ILogger<UsersController> logger, UsersContext context, IMapper mapper)
    {
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserState)
            .Include(u => u.UserGroup)
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user is null || user.UserState.Code == UserState.State.Blocked)
            return NotFound("User not found");

        var userInfo = _mapper.Map<UserInfo>(user);

        return Ok(userInfo);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRegistrationInfo>> Post(UserRegistrationInfo userRegistrationInfo)
    {
        // TODO add check for user creation in last 5 seconds 

        var user = await _context.Users
            .Include(u => u.UserState)
            .FirstOrDefaultAsync(u => u.UserState.Code == UserState.State.Active && u.Login == userRegistrationInfo.Login);

        if (user is not null)
            return BadRequest("User with this name already exists");

        if (!Enum.TryParse(userRegistrationInfo.Group, out UserGroup.Group userGroup))
            return BadRequest("Unknown user group");
        
        if (userGroup == UserGroup.Group.Admin)
        {
            var admin = await _context.Users
                .Include(u => u.UserGroup)
                .Include(u => u.UserState)
                .FirstOrDefaultAsync(u =>
                    u.UserState.Code == UserState.State.Active && u.UserGroup.Code == UserGroup.Group.Admin);
            if (admin is not null)
                return BadRequest("Admin already exits");
        }
        
        var newUser = new User()
        {
            Login = userRegistrationInfo.Login,
            Password = userRegistrationInfo.Password, // TODO хеширование
            UserState = new UserState() { Code = UserState.State.Active },
            UserGroup = new UserGroup() { Code = userGroup }
        };
        
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return Created("", userRegistrationInfo);
    }
    
    [HttpDelete("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserState)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return NotFound("User not found");

        user.UserState.Code = UserState.State.Blocked;
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}