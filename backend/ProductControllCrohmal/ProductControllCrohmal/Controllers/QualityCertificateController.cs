using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/quality-certificates")]
    public class QualityCertificateController : BaseApiController
    {
        private readonly IQualityCertificateService _qualityCertificateService;

        public QualityCertificateController(IQualityCertificateService qualityCertificateService)
        {
            _qualityCertificateService = qualityCertificateService;
        }

        [HttpPost("batch/{batchId:int}")]
        public async Task<ActionResult<QualityCertificateDTO>> GenerateCertificate(
            int batchId,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var certificate = await _qualityCertificateService
                    .GenerateCertificateAsync(batchId, userId, cancellationToken);

                return Ok(certificate);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("batch/{batchId:int}")]
        public async Task<ActionResult<QualityCertificateDTO>> GetCertificateByBatchId(
            int batchId,
            CancellationToken cancellationToken)
        {
            try
            {
                var certificate = await _qualityCertificateService
                    .GetCertificateByBatchIdAsync(batchId, cancellationToken);

                if (certificate is null)
                    return NotFound(new { message = "Сертификат качества для партии не найден." });

                return Ok(certificate);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{certificateId:int}/pdf")]
        public async Task<IActionResult> ExportCertificateToPdf(
            int certificateId,
            CancellationToken cancellationToken)
        {
            try
            {
                var pdfBytes = await _qualityCertificateService
                    .ExportCertificateToPdfAsync(certificateId, cancellationToken);

                return File(
                    pdfBytes,
                    "application/pdf",
                    $"quality-certificate-{certificateId}.pdf");
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
