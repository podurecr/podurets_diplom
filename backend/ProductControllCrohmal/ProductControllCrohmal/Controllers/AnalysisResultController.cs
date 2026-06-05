using Domain.DTOs;
using Domain.Services.Interfaces;
using Domain.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [ApiController]
    [Route("api/analysis-result")]
    public class AnalysisResultController  : BaseApiController
    {
        private readonly IAnalysisResultService _analysisResultService;

        public AnalysisResultController(IAnalysisResultService analysisResultService)
        {
            _analysisResultService = analysisResultService;
        }

        [HttpGet("batch/{batchId:int}")]
        public async Task<ActionResult<List<AnalysisResultDTO>>> GetResultsByBatchId(
            int batchId,
            CancellationToken cancellationToken)
        {
            try
            {
                var results = await _analysisResultService
                    .GetResultsByBatchIdAsync(batchId, cancellationToken);

                return Ok(results);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("batch/{batchId:int}")]
        public async Task<IActionResult> AddResults(
            int batchId,
            [FromBody] AnalysisResultDTO dto,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _analysisResultService.AddResultsAsync(batchId, dto, userId, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("batch/{batchId:int}/complete")]
        public async Task<IActionResult> CompleateAnalys(
            int batchId,
            [FromBody] SaveBatchAnalysisRequestDTO dto,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                if (userId <= 0)
                    return BadRequest(new { message = "Не указан пользователь, который внёс анализы." });

                await _analysisResultService.CompleteBatchAnalysisAsync(
                    batchId,
                    dto,
                    userId,
                    cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpPut("{resultId:int}")]
        public async Task<IActionResult> UpdateResult(
            int resultId,
            [FromBody] AnalysisResultDTO dto,
            CancellationToken cancellationToken)
        {
            try
            {
                await _analysisResultService.UpdateResultAsync(resultId, dto, cancellationToken);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<AnalysisResultDTO>>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                var result = _analysisResultService.GetAnalysisResultsAsync(cancellationToken).Result;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("batch/{batchId:int}/finish")]
        public async Task<IActionResult> FinishBatchAnalysis(
            int batchId,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                await _analysisResultService.FinishBatchAnalysisAsync(
                    batchId,
                    userId,
                    cancellationToken);

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
