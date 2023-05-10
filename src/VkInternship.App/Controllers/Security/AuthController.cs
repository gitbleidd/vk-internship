using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VkInternship.App.Models;
using VkInternship.App.Services;

namespace VkInternship.App.Controllers.Security;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Authenticate([FromBody] AuthInfo authInfo)
    {
        var authUserResult = await _authService.AuthenticateAsync(authInfo.Login, authInfo.Password);
        return authUserResult.Match<IActionResult>(
            _ => Ok(),
            _ => Unauthorized(new ErrorResponse(ErrorResponseCode.InvalidLoginOrPassword, "Invalid login or password"))
        );
    }
}