using AuthenticationServer.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.API.Serivces.RefreshTokenRepository
{
    public class DatabaseRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthenticationDbContext _context;
        public DatabaseRefreshTokenRepository(AuthenticationDbContext context)
        {
            _context = context;
        }
        public async Task Create(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            RefreshToken refreshToken = await _context.RefreshTokens.FindAsync(id);
            if (refreshToken != null)
            {
                _context.RefreshTokens.Remove(refreshToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllTokenOfUser(Guid userId)
        {
            IEnumerable<RefreshToken> refreshTokens = await _context.RefreshTokens.Where(x => x.UserId == userId).ToListAsync();
            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
        }
    }
}
