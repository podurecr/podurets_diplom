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
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized("Невірний логін або пароль.");

            return Ok(result);
        }
    }
}
