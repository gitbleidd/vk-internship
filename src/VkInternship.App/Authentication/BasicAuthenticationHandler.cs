﻿using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;
using VkInternship.App.Models;
using VkInternship.App.Services;

namespace VkInternship.App.Authentication;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ILogger<BasicAuthenticationHandler> _logger;
    private readonly AuthService _authService;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        ILogger<BasicAuthenticationHandler> logger,
        UrlEncoder encoder,
        ISystemClock clock,
        AuthService authService) :
        base(options, loggerFactory, encoder, clock)
    {
        _logger = logger;
        _authService = authService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Пропустить аутентификацию, если endpoint содержит атрибут [AllowAnonymous]    
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            return AuthenticateResult.NoResult();
        
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");

        OneOf<UserInfo, Error> authUserResult;
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
            var login = credentials[0];
            var password = credentials[1];
            authUserResult = await _authService.AuthenticateAsync(login, password);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Invalid Authorization Header");
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        return authUserResult.Match<AuthenticateResult>(
            userInfo =>
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userInfo.Login),
                    new Claim(ClaimTypes.Role, userInfo.Group)
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            },
            _ => AuthenticateResult.Fail("Invalid Username or Password")
        );
    }
}