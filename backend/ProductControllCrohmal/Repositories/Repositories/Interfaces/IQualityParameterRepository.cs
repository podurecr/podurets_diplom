using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IQualityParameterRepository : IRepository<QualityParameter>
    {
        Task<IReadOnlyList<QualityParameter>> GetActiveAsync(CancellationToken cancellationToken = default);
    }
}
