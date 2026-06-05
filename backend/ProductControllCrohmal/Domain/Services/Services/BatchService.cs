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
        private readonly IProductRepository productRepository;
        private readonly IUserRepository userRepository;

        public BatchService(IBatchRepository batchRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            this.batchRepository = batchRepository;
            this.productRepository = productRepository;
            this.userRepository = userRepository;
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

            var batchesDTO = batches
                .Select(EntityToDTOMapper.ToBatchDTO)
                .ToList();

            foreach(BatchDTO batch in batchesDTO)
            {
                batch.Product = EntityToDTOMapper.ToProductDTO(productRepository.GetByIdAsync(batch.ProductId).Result);
                batch.CreatedByUser = EntityToDTOMapper.ToUserDTO(userRepository.GetByIdAsync(batch.CreatedByUserId).Result);
            }

            return batchesDTO;
        }

        public async Task<BatchDTO?> GetBatchByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var batch = await batchRepository.GetWithDetailsAsync(id, cancellationToken);

            if (batch is null)
                return null;

            var dto = EntityToDTOMapper.ToBatchDTO(batch);

            dto.Product = EntityToDTOMapper.ToProductDTO(productRepository.GetByIdAsync(batch.ProductId).Result);
            dto.CreatedByUser = EntityToDTOMapper.ToUserDTO(userRepository.GetByIdAsync(batch.CreatedByUserId).Result);
            return dto;
        }

        public async Task<BatchDTO> CreateBatchAsync(
            BatchDTO dto, CancellationToken cancellationToken = default)
        {

            var existingBatch = await batchRepository.GetByBatchNumberAsync(
                dto.BatchNumber.Trim(),
                cancellationToken);


            var batch = new Batch
            {
                BatchNumber = dto.BatchNumber.Trim(),
                ProductionDate = dto.ProductionDate == default
                    ? DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                    : DateTime.SpecifyKind(dto.ProductionDate, DateTimeKind.Utc),
                Quantity = dto.Quantity,
                Unit = string.IsNullOrWhiteSpace(dto.Unit)
                    ? "кг"
                    : dto.Unit.Trim(),
                ProductionLine = dto.ProductionLine,
                Status = BatchStatus.Registered,
                CreatedAt = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                ProductId = dto.ProductId,
                CreatedByUserId = dto.CreatedByUserId
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

            var existingBatch = await batchRepository.GetByBatchNumberAsync(
                dto.BatchNumber.Trim(),
                cancellationToken);


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


            batch.Status = status;

            batchRepository.Update(batch);
            await batchRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
