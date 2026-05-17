using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Enums;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/quality-assessments")]
    public class QualityAssessmentController : BaseApiController
    {
        private readonly IQualityAssessmentService _qualityAssessmentService;

        public QualityAssessmentController(IQualityAssessmentService qualityAssessmentService)
        {
            _qualityAssessmentService = qualityAssessmentService;
        }

        [HttpGet("batch/{batchId:int}")]
        public async Task<ActionResult<QualityAssessmentDTO>> GetAssessmentByBatchId(
            int batchId,
            CancellationToken cancellationToken)
        {
            try
            {
                var assessment = await _qualityAssessmentService
                    .GetAssessmentByBatchIdAsync(batchId, cancellationToken);

                if (assessment is null)
                    return NotFound(new { message = "Оценка качества для партии не найдена." });

                return Ok(assessment);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("batch/{batchId:int}")]
        public async Task<ActionResult<QualityAssessmentDTO>> AssessBatch(
            int batchId,
            [FromBody] QualityAssessmentDTO dto,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var assessment = await _qualityAssessmentService
                    .AssessBatchAsync(batchId, dto, userId, cancellationToken);

                return Ok(assessment);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("batch/{batchId:int}/calculated-status")]
        public async Task<ActionResult<BatchStatus>> CalculateBatchStatus(
            int batchId,
            CancellationToken cancellationToken)
        {
            try
            {
                var status = await _qualityAssessmentService
                    .CalculateBatchStatusAsync(batchId, cancellationToken);

                return Ok(status);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
