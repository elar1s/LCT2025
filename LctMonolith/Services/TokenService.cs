using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LctMonolith.Application.Options;
using LctMonolith.Domain.Entities;
using LctMonolith.Infrastructure.UnitOfWork;
using LctMonolith.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace LctMonolith.Services;

/// <summary>
/// Issues and refreshes JWT + refresh tokens.
/// </summary>
public class TokenService : ITokenService
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtOptions _options;
    private readonly SigningCredentials _creds;

    public TokenService(IUnitOfWork uow, UserManager<AppUser> userManager, IOptions<JwtOptions> options)
    {
        _uow = uow;
        _userManager = userManager;
        _options = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public async Task<TokenPair> IssueAsync(AppUser user, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var accessExp = now.AddMinutes(_options.AccessTokenMinutes);
        var refreshExp = now.AddDays(_options.RefreshTokenDays);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: accessExp,
            signingCredentials: _creds);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = GenerateSecureToken();
        var rt = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = refreshExp
        };
        await _uow.RefreshTokens.AddAsync(rt, ct);
        await _uow.SaveChangesAsync(ct);
        return new TokenPair(accessToken, accessExp, refreshToken, refreshExp);
    }

    public async Task<TokenPair> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await _uow.RefreshTokens.Query(r => r.Token == refreshToken).FirstOrDefaultAsync(ct);
        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
            throw new SecurityTokenException("Invalid refresh token");
        var user = await _userManager.FindByIdAsync(token.UserId.ToString()) ?? throw new SecurityTokenException("User not found");
        token.IsRevoked = true; // rotate
        await _uow.SaveChangesAsync(ct);
        return await IssueAsync(user, ct);
    }

    public async Task RevokeAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await _uow.RefreshTokens.Query(r => r.Token == refreshToken).FirstOrDefaultAsync(ct);
        if (token == null) return; // idempotent
        token.IsRevoked = true;
        await _uow.SaveChangesAsync(ct);
    }

    private static string GenerateSecureToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}

internal static class EfAsyncExtensions
{
    public static Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> query, CancellationToken ct = default) => Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(query, ct);
}
