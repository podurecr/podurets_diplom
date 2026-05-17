using Domain.DTOs;
using Domain.Mappers;
using Domain.Security;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Repositories;

namespace Domain.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserRepository userRepository;

        public AuthService(UserRepository userService)
        {
            this.userRepository = userService;
        }

        public async Task<UserDTO?> GetCurrentUserAsync(
           int userId,
           CancellationToken cancellationToken = default)
        {
            var user = await userRepository.GetWithRoleAsync(userId, cancellationToken);

            if (user is null)
                return null;

            return EntityToDTOMapper.ToUserDTO(user);
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto dto,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.userLogin))
                throw new UnauthorizedAccessException("Логин обязателен.");

            if (string.IsNullOrWhiteSpace(dto.userPassword))
                throw new UnauthorizedAccessException("Пароль обязателен.");

            var user = await userRepository.GetByLoginAsync(dto.userLogin.Trim(), cancellationToken);

            if (user is null)
                throw new UnauthorizedAccessException("Неверный логин или пароль.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Пользователь деактивирован.");

            var passwordIsValid = PasswordHasher.Verify(dto.userPassword, user.PasswordHash);

            if (!passwordIsValid)
                throw new UnauthorizedAccessException("Неверный логин или пароль.");

            return new LoginResponseDto
            {
                User = EntityToDTOMapper.ToUserDTO(user),
                Token = null,
                TokenExpiresAt = null
            };
        }
    }
}
