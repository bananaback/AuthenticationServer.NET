using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Serivces.RefreshTokenRepository
{
    public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public Task Create(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid();
            _refreshTokens.Add(refreshToken);
            return Task.CompletedTask;
        }

        public Task Delete(Guid id)
        {
            _refreshTokens.RemoveAll(r => r.Id == id);
            return Task.CompletedTask;
        }

        public Task DeleteAllTokenOfUser(Guid userId)
        {
            _refreshTokens.RemoveAll(r => r.UserId == userId);
            return Task.CompletedTask;
        }

        public Task<RefreshToken> GetByToken(string token)
        {
            RefreshToken refreshToken = _refreshTokens.FirstOrDefault(t => t.Token == token);
            return Task.FromResult(refreshToken);
        }
    }
}
