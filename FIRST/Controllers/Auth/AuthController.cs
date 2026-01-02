using FIRST.DTOs.Auth;
using FIRST.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FIRST.Controllers.Auth;

[Route("api/auth")]
public class AuthController : BaseApiController
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("join")]
    public async Task<IActionResult> Join([FromBody] RegisterDto dto)
    {
        var user = await _auth.RegisterAsync(dto);

        return ApiCreated($"/api/auth/users/{user.Id}", new
        {
            user.Id,
            user.FullName,
            user.Email
        }, "User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var ua = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var tokens = await _auth.LoginAsync(dto, ua, ip);
        if (tokens == null) return ApiBadRequest("Invalid credentials");

        return ApiOk(tokens, "Logged in");
    }

    public class RefreshRequest { public string RefreshToken { get; set; } = ""; }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest body)
    {
        var ua = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var tokens = await _auth.RefreshAsync(body.RefreshToken, ua, ip);
        if (tokens == null) return ApiBadRequest("Invalid refresh token");

        return ApiOk(tokens, "Token refreshed");
    }

    public class LogoutRequest { public string RefreshToken { get; set; } = ""; }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest body)
    {
        var ok = await _auth.LogoutAsync(body.RefreshToken);
        if (!ok) return ApiBadRequest("Invalid refresh token");

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub");

        var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email");

        var data = new
        {
            userId,
            email,
            name = User.FindFirstValue("name"),
            isAuth = User.Identity?.IsAuthenticated ?? false,
            Roles = new[] { "ADMIN" }
        };

        return ApiOk(data, "User retrieved");
    }
}
