using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Repositories;

namespace Domain.Services.Services
{
    public class ProductQualitySpecificationService : IProductQualitySpecificationService
    {
        private readonly ProductQualitySpecificationRepository productQualitySpecificationRepository;

        public ProductQualitySpecificationService(ProductQualitySpecificationRepository productQualitySpecificationRepository)
        {
            this.productQualitySpecificationRepository = productQualitySpecificationRepository;
        }

        public async Task<List<ProductQualitySpecificationDTO>> GetSpecificationsByProductIdAsync(
           int productId,
           CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
                throw new ArgumentException("Некорректный ID продукта.");

            var specifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(productId, cancellationToken);

            return specifications
                .Select(EntityToDTOMapper.ToSpecificationDTO)
                .ToList();
        }

        public async Task<ProductQualitySpecificationDTO> CreateSpecificationAsync(
            ProductQualitySpecificationDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.ProductId <= 0)
                throw new InvalidOperationException("Не указан продукт.");

            if (dto.QualityParameterId <= 0)
                throw new InvalidOperationException("Не указан показатель качества.");

            if (dto.MinValue.HasValue && dto.MaxValue.HasValue && dto.MinValue > dto.MaxValue)
                throw new InvalidOperationException("Минимальное значение не может быть больше максимального.");

            var existingSpecifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(dto.ProductId, cancellationToken);

            var alreadyExists = existingSpecifications.Any(x =>
                x.QualityParameterId == dto.QualityParameterId);

            if (alreadyExists)
                throw new InvalidOperationException("Спецификация для этого показателя качества уже существует у выбранного продукта.");

            var specification = new ProductQualitySpecification
            {
                ProductId = dto.ProductId,
                QualityParameterId = dto.QualityParameterId,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
                TextNorm = dto.TextNorm,
                IsRequired = dto.IsRequired
            };

            await productQualitySpecificationRepository.AddAsync(specification, cancellationToken);
            await productQualitySpecificationRepository.SaveChangesAsync(cancellationToken);

            return EntityToDTOMapper.ToSpecificationDTO(specification);
        }

        public async Task UpdateSpecificationAsync(
            int id,
            ProductQualitySpecificationDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (id <= 0)
                throw new ArgumentException("Некорректный ID спецификации.");

            if (dto.ProductId <= 0)
                throw new InvalidOperationException("Не указан продукт.");

            if (dto.QualityParameterId <= 0)
                throw new InvalidOperationException("Не указан показатель качества.");

            if (dto.MinValue.HasValue && dto.MaxValue.HasValue && dto.MinValue > dto.MaxValue)
                throw new InvalidOperationException("Минимальное значение не может быть больше максимального.");

            var specification = await productQualitySpecificationRepository
                .GetByIdAsync(id, cancellationToken);

            if (specification is null)
                throw new KeyNotFoundException("Спецификация качества не найдена.");

            var existingSpecifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(dto.ProductId, cancellationToken);

            var duplicateExists = existingSpecifications.Any(x =>
                x.Id != id &&
                x.ProductId == dto.ProductId &&
                x.QualityParameterId == dto.QualityParameterId);

            if (duplicateExists)
                throw new InvalidOperationException("Спецификация для этого показателя качества уже существует у выбранного продукта.");

            specification.ProductId = dto.ProductId;
            specification.QualityParameterId = dto.QualityParameterId;
            specification.MinValue = dto.MinValue;
            specification.MaxValue = dto.MaxValue;
            specification.TextNorm = dto.TextNorm;
            specification.IsRequired = dto.IsRequired;

            productQualitySpecificationRepository.Update(specification);
            await productQualitySpecificationRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
