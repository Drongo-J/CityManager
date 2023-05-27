using CityManagerApi.Data;
using CityManagerApi.Dtos;
using CityManagerApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityManagerApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _authRepository;
        private IConfiguration _configuration;

        public AuthController(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegistrationDto dto)
        {
            if (await _authRepository.UserExists(dto.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToCreate = new User()
            {
                Username = dto.Username,
            };

            await _authRepository.Register(userToCreate, dto.Password);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto dto)
        {
            var user = await _authRepository.Login(dto.Username, dto.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Token").Value);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username)
                    }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(tokenString);
        }
    }
}
