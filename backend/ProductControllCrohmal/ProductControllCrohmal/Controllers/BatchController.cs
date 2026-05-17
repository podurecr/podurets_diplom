using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Repositories.Enums;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/batches")]
    public class BatchController : BaseApiController
    {
        private readonly IBatchService _batchService;

        public BatchController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BatchDTO>>> GetBatches(
            [FromQuery] BatchDTO filter,
            CancellationToken cancellationToken)
        {
            try
            {
                var batches = await _batchService.GetBatchesAsync(filter, cancellationToken);
                return Ok(batches);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BatchDTO>> GetBatchById(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var batch = await _batchService.GetBatchByIdAsync(id, cancellationToken);

                if (batch is null)
                    return NotFound(new { message = "Партия не найдена." });

                return Ok(batch);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<BatchDTO>> CreateBatch(
            [FromBody] BatchDTO dto,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var createdBatch = await _batchService.CreateBatchAsync(dto, userId, cancellationToken);

                return CreatedAtAction(
                    nameof(GetBatchById),
                    new { id = createdBatch.Id },
                    createdBatch);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<BatchDTO>> UpdateBatch(
            int id,
            [FromBody] BatchDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                var updatedBatch = await _batchService.UpdateBatchAsync(id, dto, cancellationToken);
                return Ok(updatedBatch);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(
            int id,
            [FromQuery] BatchStatus status,
            CancellationToken cancellationToken)
        {
            try
            {
                await _batchService.ChangeStatusAsync(id, status, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
