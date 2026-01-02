using FIRST.Data;
using FIRST.DTOs.Auth;
using FIRST.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FIRST.Services.Auth;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<User> RegisterAsync(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLower();

        if (await _db.Users.AnyAsync(u => u.Email == email))
            throw new InvalidOperationException("Email already used");

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, string userAgent, string ?ip)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.IsActive);

        if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

        var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if( ! ok ) throw new UnauthorizedAccessException("Invalid credentials");
        return await CreateTokensForUserAsync(user, userAgent, ip);
    }

    public async Task<AuthResponseDto?> RefreshAsync(string refreshTokenRaw, string? userAgent, string? ip)
    {
        var tokenHash = Sha256(refreshTokenRaw);

        var session = await _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

        if (session?.User == null) return null;
        if (session.RevokedAt != null) return null;
        if (session.ExpiresAt <= DateTime.UtcNow) return null;

        // rotate refresh token
        session.RevokedAt = DateTime.UtcNow;

        var newRefreshRaw = GenerateSecureToken();
        var newHash = Sha256(newRefreshRaw);

        session.ReplacedByTokenHash = newHash;

        var refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"]!);
        var newSession = new RefreshToken
        {
            UserId = session.UserId,
            TokenHash = newHash,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshDays),
            UserAgent = userAgent,
            IpAddress = ip
        };

        _db.RefreshTokens.Add(newSession);
        await _db.SaveChangesAsync();

        var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"]!);
        var accessToken = GenerateJwt(session.User, DateTime.UtcNow.AddMinutes(accessMinutes));

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(accessMinutes),
            RefreshToken = newRefreshRaw,
            RefreshTokenExpiresAt = newSession.ExpiresAt
        };
    }

    public async Task<bool> LogoutAsync(string refreshTokenRaw)
    {
        var tokenHash = Sha256(refreshTokenRaw);

        var session = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == tokenHash);
        if (session == null) return false;

        session.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<AuthResponseDto> CreateTokensForUserAsync(User user, string? userAgent, string? ip)
    {
        var accessMinutes = int.Parse(_config["Jwt:AccessTokenMinutes"]!);
        var refreshDays = int.Parse(_config["Jwt:RefreshTokenDays"]!);

        var accessExp = DateTime.UtcNow.AddMinutes(accessMinutes);
        var refreshExp = DateTime.UtcNow.AddDays(refreshDays);

        var accessToken = GenerateJwt(user, accessExp);

        var refreshRaw = GenerateSecureToken();
        var refreshHash = Sha256(refreshRaw);

        var session = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshHash,
            ExpiresAt = refreshExp,
            UserAgent = userAgent,
            IpAddress = ip
        };

        _db.RefreshTokens.Add(session);
        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessExp,
            RefreshToken = refreshRaw,
            RefreshTokenExpiresAt = refreshExp
        };
    }

    private string GenerateJwt(User user, DateTime expiresAt)
    {
        var jwt = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.FullName)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string Sha256(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToBase64String(bytes);
    }
}
