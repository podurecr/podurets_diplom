using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Repositories.Repositories.Repositories
{
    public class QualityParameterRepository : Repository<QualityParameter>, IQualityParameterRepository
    {
        public QualityParameterRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<QualityParameter>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
