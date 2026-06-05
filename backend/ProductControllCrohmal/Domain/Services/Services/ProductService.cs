using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;

namespace Domain.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;

        public ProductService(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<List<ProductDTO>> GetProductsAsync(
                   CancellationToken cancellationToken = default)
        {
            var products = await productRepository.GetAllAsync(cancellationToken);

            return products
                .OrderBy(x => x.Name)
                .Select(EntityToDTOMapper.ToProductDTO)
                .ToList();
        }

        public async Task<ProductDTO?> GetProductByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {

            var product = await productRepository.GetByIdAsync(id, cancellationToken);

            if (product is null)
                return null;

            return EntityToDTOMapper.ToProductDTO(product);
        }

        public async Task<ProductDTO> CreateProductAsync(
            ProductDTO dto,
            CancellationToken cancellationToken = default)
        {

            var existingProduct = await productRepository.GetByCodeAsync(
                dto.Code.Trim(),
                cancellationToken);


            var product = new Product
            {
                Code = dto.Code.Trim(),
                Name = dto.Name.Trim(),
                Unit = dto.Unit.Trim(),
                Description = dto.Description,
                IsActive = true
            };

            await productRepository.AddAsync(product, cancellationToken);
            await productRepository.SaveChangesAsync(cancellationToken);

            return EntityToDTOMapper.ToProductDTO(product);
        }

        public async Task<ProductDTO> UpdateProductAsync(
            int id,
            ProductDTO dto,
            CancellationToken cancellationToken = default)
        {
            var product = await productRepository.GetByIdAsync(id, cancellationToken);



            var existingProduct = await productRepository.GetByCodeAsync(
                dto.Code.Trim(),
                cancellationToken);

            product.Code = dto.Code.Trim();
            product.Name = dto.Name.Trim();
            product.Unit = dto.Unit.Trim();
            product.Description = dto.Description;
            product.IsActive = dto.IsActive;

            productRepository.Update(product);
            await productRepository.SaveChangesAsync(cancellationToken);

            return EntityToDTOMapper.ToProductDTO(product);
        }
    }
}
