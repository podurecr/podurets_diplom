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
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public async Task<List<UserDTO>> GetUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var users = await _userRepository
                .QueryNoTracking()
                .Include(x => x.Role)
                .OrderBy(x => x.FullName)
                .ToListAsync(cancellationToken);

            var list = new List<UserDTO>();

            foreach (var user in users) { 
                var dto = EntityToDTOMapper.ToUserDTO(user);
                dto.Role = EntityToDTOMapper.ToRoleDTO(roleRepository.GetByIdAsync(dto.RoleId).Result);

                list.Add(dto);
            }

            return list;
        }

        public async Task<UserDTO?> GetUserByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {

            var user = await _userRepository.GetWithRoleAsync(id, cancellationToken);

            if (user is null)
                return null;

            return EntityToDTOMapper.ToUserDTO(user);
        }

        public async Task<UserDTO> CreateUserAsync(
            UserDTO dto,
            CancellationToken cancellationToken = default)
        {

            var normalizedLogin = dto.Login.Trim();
            var normalizedEmail = dto.Email.Trim();

            var existingByLogin = await _userRepository
                .GetByLoginAsync(normalizedLogin, cancellationToken);

            if (existingByLogin is not null)
                throw new InvalidOperationException("Користувач з таким логіном вже існує.");

            var existingByEmail = await _userRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

            if (existingByEmail is not null)
                throw new InvalidOperationException("Користувач з такою поштою вже існує.");

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Login = normalizedLogin,
                Email = normalizedEmail,
                PasswordHash = PasswordHasher.HashPassword(dto.PasswordHash),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RoleId = dto.Role.Id
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
            var user = await _userRepository.GetByIdForUpdateAsync(id, cancellationToken);


            var normalizedLogin = dto.Login.Trim();
            var normalizedEmail = dto.Email.Trim();

            var existingByLogin = await _userRepository
                .GetByLoginAsync(normalizedLogin, cancellationToken);

            if (existingByLogin is not null && existingByLogin.Id != id)
                throw new InvalidOperationException("Користувач з таким логіном вже існує.");

            var existingByEmail = await _userRepository
                .GetByEmailAsync(normalizedEmail, cancellationToken);

            if (existingByEmail is not null && existingByEmail.Id != id)
                throw new InvalidOperationException("Користувач з такою поштою вже існує.");

            user.FullName = dto.FullName.Trim();
            user.Login = normalizedLogin;
            user.Email = normalizedEmail;
            user.IsActive = dto.IsActive;
            user.RoleId = dto.Role.Id;

            if (!string.IsNullOrWhiteSpace(dto.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.HashPassword(dto.PasswordHash.Trim());
            }


            await _userRepository.SaveChangesAsync(cancellationToken);

            var updatedUser = await _userRepository.GetWithRoleAsync(id, cancellationToken);

            return EntityToDTOMapper.ToUserDTO(updatedUser);
        }

        public async Task DeleteUserAsync(
            int id,
            CancellationToken cancellationToken = default)
        {

            var user = await _userRepository.GetByIdAsync(id, cancellationToken);


            user.IsActive = false;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }      
}
