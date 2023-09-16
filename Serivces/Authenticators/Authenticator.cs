using AuthenticationServer.API.Models;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Serivces.RefreshTokenRepository;
using AuthenticationServer.API.Serivces.TokenGenerators;

namespace AuthenticationServer.API.Serivces.Authenticators
{
    public class Authenticator
    {
        private readonly AccessTokenGenerator _accessTokenGenerator;
        private readonly RefreshTokenGenerator _refreshTokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Authenticator(AccessTokenGenerator accessTokenGenerator, RefreshTokenGenerator refreshTokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthenticatedUserResponse> Authenticate(User user)
        {
            string accessToken = _accessTokenGenerator.GenerateToken(user);
            string refreshToken = _refreshTokenGenerator.GenerateToken();

            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };

            await _refreshTokenRepository.Create(refreshTokenDTO);

            return new AuthenticatedUserResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
