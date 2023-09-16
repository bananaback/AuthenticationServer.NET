using AuthenticationServer.API.Models;

namespace AuthenticationServer.API.Serivces.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly AuthenticationConfiguration _authenticationConfiguration;
        private readonly TokenGenerator _tokenGenerator;

        public RefreshTokenGenerator(AuthenticationConfiguration authenticationConfiguration, TokenGenerator tokenGenerator)
        {
            _authenticationConfiguration = authenticationConfiguration;
            _tokenGenerator = tokenGenerator;
        }

        public string GenerateToken()
        {
            return _tokenGenerator.GenerateToken(
                _authenticationConfiguration.RefreshTokenSecret,
                _authenticationConfiguration.Issuer,
                _authenticationConfiguration.Audience,
                _authenticationConfiguration.RefreshTokenExpirationMinutes);
        }
    }
}
