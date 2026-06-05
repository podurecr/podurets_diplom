using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly ApplicationDbContext context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<User?> GetWithRoleAsync(int id, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }


        public async Task<User?> GetByIdForUpdateAsync(int userId, CancellationToken cancellationToken)
        {
            return await context.Users
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        }
    }
}
