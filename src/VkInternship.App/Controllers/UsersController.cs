using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkInternship.App.Models;
using VkInternship.App.Services;

namespace VkInternship.App.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get(int id)
    {
        var getUserResult = await _userService.GetUserAsync(id);
        return getUserResult.Match<ActionResult>(
            Ok,
            _ => NotFound(new ErrorResponse(ErrorResponseCode.UserNotFound, "User not found")));
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get([FromQuery] int limit, [FromQuery] int offset)
    {
        var getUserResult = await _userService.GetUserAsync(limit, offset);
        return getUserResult.Match<ActionResult>(Ok);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserRegistrationInfo>> Post(UserRegistrationInfo userRegistrationInfo)
    {
        var createUserResult = await _userService.CreateUserAsync(userRegistrationInfo);

        return createUserResult.Match<ActionResult>(
            user => Created("", user),
            _ => BadRequest(new ErrorResponse(ErrorResponseCode.UserNameTaken, "User with this name already exists")),
            _ => BadRequest(new ErrorResponse(ErrorResponseCode.UnknownUserGroup, "Unknown user group")),
            _ => BadRequest(new ErrorResponse(ErrorResponseCode.AdminAlreadyExists, "Admin already exits")));
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(int id)
    {
        var deleteUserResult = await _userService.DeleteUserAsync(id);

        return deleteUserResult.Match<ActionResult>(
            _ => NoContent(),
            _ => NotFound(new ErrorResponse(ErrorResponseCode.UserNotFound, "User not found")));
    }
}