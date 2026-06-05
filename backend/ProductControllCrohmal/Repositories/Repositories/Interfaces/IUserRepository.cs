using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<User?> GetWithRoleAsync(int id, CancellationToken cancellationToken = default);
        Task<User?> GetByIdForUpdateAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
