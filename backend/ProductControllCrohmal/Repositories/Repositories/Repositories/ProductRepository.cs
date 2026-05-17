using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        }

        public async Task<IReadOnlyList<Product>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetWithSpecificationsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.QualitySpecifications)
                    .ThenInclude(x => x.QualityParameter)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
    }
}
