using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Repositories;

namespace Domain.Services.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductRepository productRepository;

        public ProductService(ProductRepository productRepository)
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
            if (id <= 0)
                throw new ArgumentException("Некорректный ID продукта.");

            var product = await productRepository.GetByIdAsync(id, cancellationToken);

            if (product is null)
                return null;

            return EntityToDTOMapper.ToProductDTO(product);
        }

        public async Task<ProductDTO> CreateProductAsync(
            ProductDTO dto,
            CancellationToken cancellationToken = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new InvalidOperationException("Код продукта обязателен.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new InvalidOperationException("Название продукта обязательно.");

            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new InvalidOperationException("Единица измерения обязательна.");

            var existingProduct = await productRepository.GetByCodeAsync(
                dto.Code.Trim(),
                cancellationToken);

            if (existingProduct is not null)
                throw new InvalidOperationException("Продукт с таким кодом уже существует.");

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
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (id <= 0)
                throw new ArgumentException("Некорректный ID продукта.");

            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new InvalidOperationException("Код продукта обязателен.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new InvalidOperationException("Название продукта обязательно.");

            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new InvalidOperationException("Единица измерения обязательна.");

            var product = await productRepository.GetByIdAsync(id, cancellationToken);

            if (product is null)
                throw new KeyNotFoundException("Продукт не найден.");

            var existingProduct = await productRepository.GetByCodeAsync(
                dto.Code.Trim(),
                cancellationToken);

            if (existingProduct is not null && existingProduct.Id != id)
                throw new InvalidOperationException("Продукт с таким кодом уже существует.");

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
