using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Interfaces;

namespace Domain.Services.Services
{
    public class BatchService : IBatchService
    {
        private readonly IBatchRepository batchRepository;

        public BatchService(IBatchRepository batchRepository)
        {
            this.batchRepository = batchRepository;
        }

        public async Task<List<BatchDTO>> GetBatchesAsync(
                   BatchDTO filter,
                   CancellationToken cancellationToken = default)
        {
            var query = batchRepository
                .QueryNoTracking()
                .AsQueryable();

            if (filter is not null)
            {
                if (filter.Id > 0)
                    query = query.Where(x => x.Id == filter.Id);

                if (!string.IsNullOrWhiteSpace(filter.BatchNumber))
                    query = query.Where(x => x.BatchNumber.Contains(filter.BatchNumber.Trim()));

                if (filter.ProductId > 0)
                    query = query.Where(x => x.ProductId == filter.ProductId);

                if (filter.CreatedByUserId > 0)
                    query = query.Where(x => x.CreatedByUserId == filter.CreatedByUserId);

                if (filter.ProductionDate != default)
                    query = query.Where(x => x.ProductionDate.Date == filter.ProductionDate.Date);
            }

            var batches = await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return batches
                .Select(EntityToDTOMapper.ToBatchDTO)
                .ToList();
        }

        public async Task<BatchDTO?> GetBatchByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var batch = await batchRepository.GetWithDetailsAsync(id, cancellationToken);

            if (batch is null)
                return null;

            return EntityToDTOMapper.ToBatchDTO(batch);
        }

        public async Task<BatchDTO> CreateBatchAsync(
            BatchDTO dto,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.BatchNumber))
                throw new InvalidOperationException("Номер партии обязателен.");

            if (dto.ProductId <= 0)
                throw new InvalidOperationException("Не указан продукт партии.");

            if (dto.Quantity <= 0)
                throw new InvalidOperationException("Количество должно быть больше нуля.");

            var existingBatch = await batchRepository.GetByBatchNumberAsync(
                dto.BatchNumber.Trim(),
                cancellationToken);

            if (existingBatch is not null)
                throw new InvalidOperationException("Партия с таким номером уже существует.");

            var batch = new Batch
            {
                BatchNumber = dto.BatchNumber.Trim(),
                ProductionDate = dto.ProductionDate == default
                    ? DateTime.UtcNow
                    : dto.ProductionDate,
                Quantity = dto.Quantity,
                Unit = string.IsNullOrWhiteSpace(dto.Unit)
                    ? "кг"
                    : dto.Unit.Trim(),
                ProductionLine = dto.ProductionLine,
                Status = BatchStatus.Registered,
                CreatedAt = DateTime.UtcNow,
                ProductId = dto.ProductId,
                CreatedByUserId = userId
            };

            await batchRepository.AddAsync(batch, cancellationToken);
            await batchRepository.SaveChangesAsync(cancellationToken);

            return EntityToDTOMapper.ToBatchDTO(batch);
        }

        public async Task<BatchDTO> UpdateBatchAsync(
            int id,
            BatchDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var batch = await batchRepository.GetByIdAsync(id, cancellationToken);

            if (batch is null)
                throw new KeyNotFoundException("Партия не найдена.");

            if (string.IsNullOrWhiteSpace(dto.BatchNumber))
                throw new InvalidOperationException("Номер партии обязателен.");

            if (dto.ProductId <= 0)
                throw new InvalidOperationException("Не указан продукт партии.");

            if (dto.Quantity <= 0)
                throw new InvalidOperationException("Количество должно быть больше нуля.");

            var existingBatch = await batchRepository.GetByBatchNumberAsync(
                dto.BatchNumber.Trim(),
                cancellationToken);

            if (existingBatch is not null && existingBatch.Id != id)
                throw new InvalidOperationException("Партия с таким номером уже существует.");

            batch.BatchNumber = dto.BatchNumber.Trim();
            batch.ProductionDate = dto.ProductionDate == default
                ? batch.ProductionDate
                : dto.ProductionDate;
            batch.Quantity = dto.Quantity;
            batch.Unit = string.IsNullOrWhiteSpace(dto.Unit)
                ? batch.Unit
                : dto.Unit.Trim();
            batch.ProductionLine = dto.ProductionLine;
            batch.ProductId = dto.ProductId;

            batchRepository.Update(batch);
            await batchRepository.SaveChangesAsync(cancellationToken);

            return EntityToDTOMapper.ToBatchDTO(batch);
        }

        public async Task ChangeStatusAsync(
            int batchId,
            BatchStatus status,
            CancellationToken cancellationToken = default)
        {
            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);

            if (batch is null)
                throw new KeyNotFoundException("Партия не найдена.");

            batch.Status = status;

            batchRepository.Update(batch);
            await batchRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
