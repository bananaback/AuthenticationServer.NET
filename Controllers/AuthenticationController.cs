using AuthenticationServer.API.Models;
using AuthenticationServer.API.Models.Request;
using AuthenticationServer.API.Models.Requests;
using AuthenticationServer.API.Models.Responses;
using AuthenticationServer.API.Serivces.Authenticators;
using AuthenticationServer.API.Serivces.PasswordHasher;
using AuthenticationServer.API.Serivces.RefreshTokenRepository;
using AuthenticationServer.API.Serivces.TokenValidators;
using AuthenticationServer.API.Serivces.UserRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthenticationServer.API.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly Authenticator _authenticator;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationController(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            Authenticator authenticator,
            RefreshTokenValidator refreshTokenValidator,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _authenticator = authenticator;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest(new ErrorResponse("Password does not match confirm password."));
            }
            User existingUserByEmail = await _userRepository.GetByEmail(registerRequest.Email);
            if (existingUserByEmail != null)
            {
                return Conflict(new ErrorResponse("Email already exist."));
            }
            User existingUserByUsername = await _userRepository.GetByUsername(registerRequest.Username);
            if (existingUserByUsername != null)
            {
                return Conflict(new ErrorResponse("Username already exist."));
            }
            string passwordHash = _passwordHasher.HashPassword(registerRequest.Password);
            User registrationUser = new User()
            {
                Email = registerRequest.Email,
                Username = registerRequest.Username,
                PasswordHash = passwordHash
            };
            await _userRepository.Create(registrationUser);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            User user = await _userRepository.GetByUsername(loginRequest.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            bool isCorrectPassword = _passwordHasher.VerifyPassword(loginRequest.Password, user.PasswordHash);
            if (!isCorrectPassword)
            {
                return Unauthorized();
            }

            AuthenticatedUserResponse response = await _authenticator.Authenticate(user);
            return Ok(response);

        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            bool isValidRefreshToken = _refreshTokenValidator.Validate(refreshRequest.RefreshToken);
            if (!isValidRefreshToken)
            {
                return BadRequest(new ErrorResponse("Invalid refresh token."));
            }

            RefreshToken refreshTokenDTO = await _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);
            if (refreshTokenDTO == null)
            {
                return BadRequest(new ErrorResponse("Invalid refresh token"));
            }

            await _refreshTokenRepository.Delete(refreshTokenDTO.Id);

            User user = await _userRepository.GetById(refreshTokenDTO.UserId);
            if (user == null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            AuthenticatedUserResponse response = await _authenticator.Authenticate(user);
            return Ok(response);

        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            string rawUserId = HttpContext.User.FindFirstValue("id");

            if (!Guid.TryParse(rawUserId, out Guid userId))
            {
                return Unauthorized();
            }

            await _refreshTokenRepository.DeleteAllTokenOfUser(userId);

            return NoContent();
        }

        private IActionResult BadRequestModelState()
        {
            IEnumerable<string> errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ErrorResponse(errorMessages));
        }
    }
}
