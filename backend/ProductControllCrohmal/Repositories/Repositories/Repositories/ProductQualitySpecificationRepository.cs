using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class ProductQualitySpecificationRepository
            : Repository<ProductQualitySpecification>, IProductQualitySpecificationRepository
    {
        public ProductQualitySpecificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<ProductQualitySpecification>> GetByProductIdAsync(
            int productId,
            CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.QualityParameter)
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.QualityParameter!.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
