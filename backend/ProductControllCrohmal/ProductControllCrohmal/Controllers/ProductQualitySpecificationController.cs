using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/quality-specifications")]
    public class ProductQualitySpecificationController : BaseApiController
    {
        private readonly IProductQualitySpecificationService _qualitySpecificationService;

        public ProductQualitySpecificationController(IProductQualitySpecificationService qualitySpecificationService)
        {
            _qualitySpecificationService = qualitySpecificationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductQualitySpecificationDTO>>> GetSpecifications()
        {
            try
            {
                var specifications = await _qualitySpecificationService.GetSpecificationsAsync();

                return Ok(specifications);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("product/{productId:int}")]
        public async Task<ActionResult<List<ProductQualitySpecificationDTO>>> GetSpecificationsByProductId(
            int productId,
            CancellationToken cancellationToken)
        {
            try
            {
                var specifications = await _qualitySpecificationService
                    .GetSpecificationsByProductIdAsync(productId, cancellationToken);

                return Ok(specifications);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductQualitySpecificationDTO>> CreateSpecification(
            [FromBody] ProductQualitySpecificationDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var createdSpecification = await _qualitySpecificationService
                    .CreateSpecificationAsync(dto, cancellationToken);

                return CreatedAtAction(
                    nameof(GetSpecificationsByProductId),
                    new { productId = createdSpecification.ProductId },
                    createdSpecification);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSpecification(
            int id,
            [FromBody] ProductQualitySpecificationDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                await _qualitySpecificationService.UpdateSpecificationAsync(id, dto, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
