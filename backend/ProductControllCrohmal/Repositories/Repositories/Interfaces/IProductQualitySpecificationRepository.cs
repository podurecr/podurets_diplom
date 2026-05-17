using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IProductQualitySpecificationRepository : IRepository<ProductQualitySpecification>
    {
        Task<IReadOnlyList<ProductQualitySpecification>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    }
}
