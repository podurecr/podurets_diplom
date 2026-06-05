using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;

namespace Domain.Services.Services
{
    public class ProductQualitySpecificationService : IProductQualitySpecificationService
    {
        private readonly IProductQualitySpecificationRepository productQualitySpecificationRepository;
        private readonly IProductRepository productRepository;
        private readonly IQualityParameterRepository qualityParameterRepository;

        public ProductQualitySpecificationService(IProductQualitySpecificationRepository productQualitySpecificationRepository, IProductRepository productRepository, IQualityParameterRepository qualityParameterRepository)
        {
            this.productQualitySpecificationRepository = productQualitySpecificationRepository;
            this.productRepository = productRepository;
            this.qualityParameterRepository = qualityParameterRepository;
        }

        public async Task<List<ProductQualitySpecificationDTO>> GetSpecificationsByProductIdAsync(
           int productId,
           CancellationToken cancellationToken = default)
        {

            var specifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(productId, cancellationToken);

            var result = new List<ProductQualitySpecificationDTO>();

            foreach (var specification in specifications) {
                var specificationDTO = EntityToDTOMapper.ToSpecificationDTO(specification);
                specificationDTO.Product = EntityToDTOMapper.ToProductDTO(await productRepository.GetByIdAsync(specification.ProductId));
                specificationDTO.QualityParameter = EntityToDTOMapper.ToQualityParameterDTO(await qualityParameterRepository.GetByIdAsync(specification.QualityParameterId));
                result.Add(specificationDTO);
            }

            return result;
        }

        public async Task<ProductQualitySpecificationDTO> CreateSpecificationAsync(
            ProductQualitySpecificationDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));


            var existingSpecifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(dto.ProductId, cancellationToken);

            var alreadyExists = existingSpecifications.Any(x =>
                x.QualityParameterId == dto.QualityParameterId);


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

            var specification = await productQualitySpecificationRepository
                .GetByIdAsync(id, cancellationToken);

            var existingSpecifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(dto.ProductId, cancellationToken);

            var duplicateExists = existingSpecifications.Any(x =>
                x.Id != id &&
                x.ProductId == dto.ProductId &&
                x.QualityParameterId == dto.QualityParameterId);

            specification.ProductId = dto.ProductId;
            specification.QualityParameterId = dto.QualityParameterId;
            specification.MinValue = dto.MinValue;
            specification.MaxValue = dto.MaxValue;
            specification.TextNorm = dto.TextNorm;
            specification.IsRequired = dto.IsRequired;

            productQualitySpecificationRepository.Update(specification);
            await productQualitySpecificationRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<ProductQualitySpecificationDTO>> GetSpecificationsAsync()
        {
            var pQS = productQualitySpecificationRepository.GetAllAsync().Result;
            var dtos = new List<ProductQualitySpecificationDTO>();

            foreach(ProductQualitySpecification entity  in pQS)
            {
                var dto = EntityToDTOMapper.ToSpecificationDTO(entity);
                dto.Product = EntityToDTOMapper.ToProductDTO(productRepository.GetByIdAsync(dto.ProductId).Result);
                dto.QualityParameter = EntityToDTOMapper.ToQualityParameterDTO(qualityParameterRepository.GetByIdAsync(dto.QualityParameterId).Result); ;
                dtos.Add(dto);
            }

            return dtos;
        }
    }
}
