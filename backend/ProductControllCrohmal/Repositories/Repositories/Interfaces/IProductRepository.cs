using Repositories.Entities;

namespace Repositories.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Product>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetWithSpecificationsAsync(int id, CancellationToken cancellationToken = default);
    }
}
