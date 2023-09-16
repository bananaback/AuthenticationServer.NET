using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Serivces.RefreshTokenRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByToken(string token);
        Task Create(RefreshToken refreshToken);
        Task Delete(Guid id);
        Task DeleteAllTokenOfUser(Guid userId);
    }
}
