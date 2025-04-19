using Ecommerce.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly EcommerceDbContext _context;

        public RefreshTokenRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> CreateAsync(RefreshToken token)
        {
            token.CreatedAt = DateTime.UtcNow;
            token.UpdatedAt = DateTime.UtcNow;

            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.Revoked.HasValue);
        }

        public async Task RevokeAsync(Guid tokenId, string replacedByToken = null)
        {
            var token = await _context.RefreshTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.Revoked = DateTime.UtcNow;
                token.ReplacedByToken = replacedByToken;
                token.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RevokeAllForUserAsync(Guid userId)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.Revoked.HasValue)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.Revoked = DateTime.UtcNow;
                token.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsActive(Guid tokenId)
        {
            var token = await _context.RefreshTokens.FindAsync(tokenId);
            return token != null &&
                   !token.Revoked.HasValue &&
                   token.Expires > DateTime.UtcNow;
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
        public async Task<RefreshToken?> GetValidTokenAsync(string token, Guid userId)
        {
            return await _context.RefreshTokens
                .Where(t => t.Token == token && t.UserId == userId && !t.IsRevoked && !t.IsExpired)
                .FirstOrDefaultAsync();
        }
    }
}