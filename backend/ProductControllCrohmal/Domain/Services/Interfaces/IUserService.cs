using Domain.DTOs;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetUsersAsync(
            CancellationToken cancellationToken = default);

        Task<UserDTO?> GetUserByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<UserDTO> CreateUserAsync(
            UserDTO dto,
            CancellationToken cancellationToken = default);

        Task<UserDTO> UpdateUserAsync(
            int id,
            UserDTO dto,
            CancellationToken cancellationToken = default);

        Task DeleteUserAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
