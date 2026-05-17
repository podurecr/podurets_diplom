using Domain.DTOs;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto dto,
            CancellationToken cancellationToken = default);

        Task<UserDTO?> GetCurrentUserAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
