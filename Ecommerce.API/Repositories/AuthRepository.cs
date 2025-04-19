
using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;
using System;

// Triển khai đầy đủ interface
public class AuthRepository : IAuthRepository
{
    private readonly EcommerceDbContext _context;

    public AuthRepository(EcommerceDbContext context)
    {
        _context = context;
    }
   
    public async Task<RefreshToken> AddRefreshTokenAsync(Guid userId, string token, DateTime expires)
    {
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            Expires = expires,
            Created = DateTime.UtcNow
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken != null)
        {
            refreshToken.Revoked = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}