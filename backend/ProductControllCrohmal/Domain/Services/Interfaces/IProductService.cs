using Domain.DTOs;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetProductsAsync(
            CancellationToken cancellationToken = default);

        Task<ProductDTO?> GetProductByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<ProductDTO> CreateProductAsync(
            ProductDTO dto,
            CancellationToken cancellationToken = default);

        Task<ProductDTO> UpdateProductAsync(
            int id,
            ProductDTO dto,
            CancellationToken cancellationToken = default);
    }
}
