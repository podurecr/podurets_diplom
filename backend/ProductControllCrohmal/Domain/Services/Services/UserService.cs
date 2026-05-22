using Domain.DTOs;
using Domain.Mappers;
using Domain.Security;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;

namespace Domain.Services.Services
{
    public class UserService : IUserService
    {
        private const string DefaultInitialPassword = "123456";

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDTO>> GetUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var users = await _userRepository
                .QueryNoTracking()
                .Include(x => x.Role)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);

            return users
                .Select(EntityToDTOMapper.ToUserDTO)
                .ToList();
        }

        public async Task<UserDTO?> GetUserByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Некорректный ID пользователя.");

            var user = await _userRepository.GetWithRoleAsync(id, cancellationToken);

            if (user is null)
                return null;

            return EntityToDTOMapper.ToUserDTO(user);
        }

        public async Task<UserDTO> CreateUserAsync(
            UserDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new InvalidOperationException("ФИО пользователя обязательно.");

            if (string.IsNullOrWhiteSpace(dto.Login))
                throw new InvalidOperationException("Логин пользователя обязателен.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new InvalidOperationException("Email пользователя обязателен.");

            if (dto.RoleId <= 0)
                throw new InvalidOperationException("Роль пользователя обязательна.");

            var normalizedLogin = dto.Login.Trim();
            var normalizedEmail = dto.Email.Trim();

            var existingByLogin = await _userRepository
                .GetByLoginAsync(normalizedLogin, cancellationToken);

            if (existingByLogin is not null)
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");

            var existingByEmail = await _userRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

            if (existingByEmail is not null)
                throw new InvalidOperationException("Пользователь с таким email уже существует.");

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Login = normalizedLogin,
                Email = normalizedEmail,
                PasswordHash = PasswordHasher.Hash(DefaultInitialPassword),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RoleId = dto.RoleId
            };

            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var createdUser = await _userRepository.GetWithRoleAsync(user.Id, cancellationToken);

            return EntityToDTOMapper.ToUserDTO(createdUser ?? user);
        }

        public async Task<UserDTO> UpdateUserAsync(
            int id,
            UserDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (id <= 0)
                throw new ArgumentException("Некорректный ID пользователя.");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new InvalidOperationException("ФИО пользователя обязательно.");

            if (string.IsNullOrWhiteSpace(dto.Login))
                throw new InvalidOperationException("Логин пользователя обязателен.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new InvalidOperationException("Email пользователя обязателен.");

            if (dto.RoleId <= 0)
                throw new InvalidOperationException("Роль пользователя обязательна.");

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user is null)
                throw new KeyNotFoundException("Пользователь не найден.");

            var normalizedLogin = dto.Login.Trim();
            var normalizedEmail = dto.Email.Trim();

            var existingByLogin = await _userRepository
                .GetByLoginAsync(normalizedLogin, cancellationToken);

            if (existingByLogin is not null && existingByLogin.Id != id)
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");

            var existingByEmail = await _userRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

            if (existingByEmail is not null && existingByEmail.Id != id)
                throw new InvalidOperationException("Пользователь с таким email уже существует.");

            user.FullName = dto.FullName.Trim();
            user.Login = normalizedLogin;
            user.Email = normalizedEmail;
            user.IsActive = dto.IsActive;
            user.RoleId = dto.RoleId;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            var updatedUser = await _userRepository.GetWithRoleAsync(id, cancellationToken);

            return EntityToDTOMapper.ToUserDTO(updatedUser ?? user);
        }

        public async Task DeleteUserAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new ArgumentException("Некорректный ID пользователя.");

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user is null)
                throw new KeyNotFoundException("Пользователь не найден.");

            // Лучше не удалять физически, потому что User связан с партиями,
            // результатами анализов, сертификатами и решениями по отгрузке.
            user.IsActive = false;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }      
}
