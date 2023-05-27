using CityManagerApi.Data;
using CityManagerApi.Dtos;
using CityManagerApi.Models;
using Microsoft.AspNetCore.Mvc;

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

    }
}
