using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/auth")]
    public class AuthController
    : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(
            [FromBody] LoginRequestDto dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.LoginAsync(dto, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("me/{userId:int}")]
        public async Task<ActionResult<UserDTO>> GetCurrentUser(
            int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync(userId, cancellationToken);

                if (user is null)
                    return NotFound(new { message = "Користувач не знайден." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
