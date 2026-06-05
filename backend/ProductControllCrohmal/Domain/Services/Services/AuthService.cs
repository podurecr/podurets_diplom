using Domain.DTOs;
using Domain.Mappers;
using Domain.Security;
using Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Domain.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IRoleRepository roleRepository)
        {
            this.userRepository = userRepository;
            _configuration = configuration;
            this.roleRepository = roleRepository;
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

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await userRepository.GetByLoginAsync(request.userLogin);

            if (user == null)
                return null;

            if (!user.IsActive)
                return null;

            var isPasswordValid = PasswordHasher.VerifyPassword(user.PasswordHash, request.userPassword);

            Console.WriteLine($"PASSWORD VALID: {isPasswordValid}");
            
            if (!isPasswordValid)
                return null;

            var tokenExpiresAt = DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(_configuration["Jwt:ExpiresMinutes"])
            );

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.Name),
            new Claim("roleName", user.Role.Name),
            new Claim("fullName", user.FullName)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: tokenExpiresAt,
                signingCredentials: credentials
            );

            UserDTO userDto = EntityToDTOMapper.ToUserDTO(user);

            userDto.Role = EntityToDTOMapper.ToRoleDTO(roleRepository.GetByIdAsync(user.RoleId).Result);

            return new LoginResponseDto
            {
                User = userDto,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiresAt = tokenExpiresAt
            };
        }
    }
}
