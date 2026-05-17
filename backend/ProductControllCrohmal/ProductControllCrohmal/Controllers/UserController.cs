 using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/users")]
    public class UserController : BaseApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> GetUsers(
            CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userService.GetUsersAsync(cancellationToken);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDTO>> GetUserById(
            int id,
            CancellationToken cancellationToken)
        { 
            try
            {
                var user = await _userService.GetUserByIdAsync(id, cancellationToken);

                if (user is null)
                    return NotFound(new { message = "Пользователь не найден." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser(
            [FromBody] UserDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(dto, cancellationToken);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = createdUser.Id },
                    createdUser);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserDTO>> UpdateUser(
            int id,
            [FromBody] UserDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, dto, cancellationToken);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _userService.DeleteUserAsync(id, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
