using AuthenticationServer.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AuthenticationServer.API.Serivces.TokenValidators
{
    public class RefreshTokenValidator
    {
        private readonly AuthenticationConfiguration _configuration;
        public RefreshTokenValidator(AuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool Validate(string refreshToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationParameters validationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.RefreshTokenSecret)),
                ValidIssuer = _configuration.Issuer,
                ValidAudience = _configuration.Audience,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
