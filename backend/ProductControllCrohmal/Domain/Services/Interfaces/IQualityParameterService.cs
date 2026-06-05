using Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IQualityParameterService
    {
        Task<List<QualityParameterDTO>> GetQualityParametersAsync(
            CancellationToken cancellationToken = default);

        Task<List<QualityParameterDTO>> GetActiveQualityParametersAsync(
            CancellationToken cancellationToken = default);

        Task<QualityParameterDTO?> GetQualityParameterByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<QualityParameterDTO> CreateQualityParameterAsync(
            QualityParameterDTO dto,
            CancellationToken cancellationToken = default);

        Task<QualityParameterDTO> UpdateQualityParameterAsync(
            int id,
            QualityParameterDTO dto,
            CancellationToken cancellationToken = default);

        Task DeleteQualityParameterAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
