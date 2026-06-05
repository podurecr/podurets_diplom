using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductControllCrohmal.Controllers
{
    [ApiController]
    [Route("api/quality-parameters")]
    public class QualityParameterController : ControllerBase
    {
        private readonly IQualityParameterService _qualityParameterService;

        public QualityParameterController(IQualityParameterService qualityParameterService)
        {
            _qualityParameterService = qualityParameterService;
        }

        [HttpGet]
        public async Task<ActionResult<List<QualityParameterDTO>>> GetQualityParameters(
            CancellationToken cancellationToken)
        {
            var result = await _qualityParameterService
                .GetQualityParametersAsync(cancellationToken);

            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<ActionResult<List<QualityParameterDTO>>> GetActiveQualityParameters(
            CancellationToken cancellationToken)
        {
            var result = await _qualityParameterService
                .GetActiveQualityParametersAsync(cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<QualityParameterDTO>> GetQualityParameterById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _qualityParameterService
                .GetQualityParameterByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound("Показник якості не знайдено.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<QualityParameterDTO>> CreateQualityParameter(
            [FromBody] QualityParameterDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _qualityParameterService
                    .CreateQualityParameterAsync(dto, cancellationToken);

                return CreatedAtAction(
                    nameof(GetQualityParameterById),
                    new { id = result.Id },
                    result
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<QualityParameterDTO>> UpdateQualityParameter(
            int id,
            [FromBody] QualityParameterDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await _qualityParameterService
                    .UpdateQualityParameterAsync(id, dto, cancellationToken);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteQualityParameter(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                await _qualityParameterService
                    .DeleteQualityParameterAsync(id, cancellationToken);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}