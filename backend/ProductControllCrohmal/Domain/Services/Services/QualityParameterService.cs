using Domain.DTOs;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services.Services
{
    public class QualityParameterService : IQualityParameterService
    {
        private readonly IQualityParameterRepository _qualityParameterRepository;

        public QualityParameterService(IQualityParameterRepository qualityParameterRepository)
        {
            _qualityParameterRepository = qualityParameterRepository;
        }

        public async Task<List<QualityParameterDTO>> GetQualityParametersAsync(
            CancellationToken cancellationToken = default)
        {
            var parameters = await _qualityParameterRepository
                .QueryNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return parameters
                .Select(ToDTO)
                .ToList();
        }

        public async Task<List<QualityParameterDTO>> GetActiveQualityParametersAsync(
            CancellationToken cancellationToken = default)
        {
            var parameters = await _qualityParameterRepository
                .GetActiveAsync(cancellationToken);

            return parameters
                .Select(ToDTO)
                .ToList();
        }

        public async Task<QualityParameterDTO?> GetQualityParameterByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {

            var parameter = await _qualityParameterRepository
                .GetByIdAsync(id, cancellationToken);

            if (parameter is null)
                return null;

            return ToDTO(parameter);
        }

        public async Task<QualityParameterDTO> CreateQualityParameterAsync(
            QualityParameterDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var normalizedName = dto.Name.Trim();

            var existingParameter = await _qualityParameterRepository
                .QueryNoTracking()
                .FirstOrDefaultAsync(x => x.Name == normalizedName, cancellationToken);


            var parameter = new QualityParameter
            {
                Name = normalizedName,
                Unit = string.IsNullOrWhiteSpace(dto.Unit) ? "—" : dto.Unit.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                IsActive = dto.IsActive
            };

            await _qualityParameterRepository.AddAsync(parameter, cancellationToken);
            await _qualityParameterRepository.SaveChangesAsync(cancellationToken);

            return ToDTO(parameter);
        }

        public async Task<QualityParameterDTO> UpdateQualityParameterAsync(
            int id,
            QualityParameterDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));


            var parameter = await _qualityParameterRepository
                .GetByIdAsync(id, cancellationToken);

            var normalizedName = dto.Name.Trim();

            var existingParameter = await _qualityParameterRepository
                .QueryNoTracking()
                .FirstOrDefaultAsync(x => x.Name == normalizedName && x.Id != id, cancellationToken);

            parameter.Name = normalizedName;
            parameter.Unit = string.IsNullOrWhiteSpace(dto.Unit) ? "—" : dto.Unit.Trim();
            parameter.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            parameter.IsActive = dto.IsActive;

            _qualityParameterRepository.Update(parameter);
            await _qualityParameterRepository.SaveChangesAsync(cancellationToken);

            return ToDTO(parameter);
        }

        public async Task DeleteQualityParameterAsync(
            int id,
            CancellationToken cancellationToken = default)
        {

            var parameter = await _qualityParameterRepository
                .GetByIdAsync(id, cancellationToken);


            parameter.IsActive = false;

            _qualityParameterRepository.Update(parameter);
            await _qualityParameterRepository.SaveChangesAsync(cancellationToken);
        }

        private static QualityParameterDTO ToDTO(QualityParameter parameter)
        {
            return new QualityParameterDTO
            {
                Id = parameter.Id,
                Name = parameter.Name,
                Unit = parameter.Unit,
                Description = parameter.Description,
                IsActive = parameter.IsActive
            };
        }
    }
}
