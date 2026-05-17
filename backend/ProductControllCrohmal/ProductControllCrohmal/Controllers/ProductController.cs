using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/products")]
    public class ProductController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductDTO>>> GetProducts(
            CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productService.GetProductsAsync(cancellationToken);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id, cancellationToken);

                if (product is null)
                    return NotFound(new { message = "Продукт не найден." });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct(
            [FromBody] ProductDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(dto, cancellationToken);

                return CreatedAtAction(
                    nameof(GetProductById),
                    new { id = createdProduct.Id },
                    createdProduct);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(
            int id,
            [FromBody] ProductDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, dto, cancellationToken);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
