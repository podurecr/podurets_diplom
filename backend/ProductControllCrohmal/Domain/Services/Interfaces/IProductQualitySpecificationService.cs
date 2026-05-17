using Domain.DTOs;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IProductQualitySpecificationService
    {
        Task<List<ProductQualitySpecificationDTO>> GetSpecificationsByProductIdAsync(
            int productId,
            CancellationToken cancellationToken = default);

        Task<ProductQualitySpecificationDTO> CreateSpecificationAsync(
            ProductQualitySpecificationDTO dto,
            CancellationToken cancellationToken = default);

        Task UpdateSpecificationAsync(
            int id,
            ProductQualitySpecificationDTO dto,
            CancellationToken cancellationToken = default);
    }
}
