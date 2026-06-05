using Domain.DTOs;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;

namespace ProductControllCrohmal.Controllers
{
    [Route("api/quality-certificates")]
    public class QualityCertificateController : BaseApiController
    {
        private readonly IQualityCertificateService qualityCertificateService;

        public QualityCertificateController(IQualityCertificateService qualityCertificateService)
        {
            this.qualityCertificateService = qualityCertificateService;
        }

        [HttpGet]
        public async Task<ActionResult<List<QualityCertificateDTO>>> GetCertificates(
            CancellationToken cancellationToken)
        {
            try
            {
                var certificates = await qualityCertificateService
                    .GetCertificatesAsync(cancellationToken);

                return Ok(certificates);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<QualityCertificateDTO>> GetCertificateById(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var certificate = await qualityCertificateService
                    .GetCertificateByIdAsync(id, cancellationToken);

                if (certificate is null)
                {
                    return NotFound(new
                    {
                        message = "Сертификат не найден."
                    });
                }

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
                var certificate = await qualityCertificateService
                    .GetCertificateByBatchIdAsync(batchId, cancellationToken);

                if (certificate is null)
                {
                    return NotFound(new
                    {
                        message = "Сертификат для партии не найден."
                    });
                }

                return Ok(certificate);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost("batch/{batchId:int}/generate")]
        public async Task<ActionResult<QualityCertificateDTO>> GenerateCertificate(
            int batchId,
            [FromQuery] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var certificate = await qualityCertificateService
                    .GenerateCertificateAsync(batchId, userId, cancellationToken);

                return Ok(certificate);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:int}/pdf")]
        public async Task<IActionResult> DownloadPdf(
            int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var certificate = await qualityCertificateService
                    .GetCertificateByIdAsync(id, cancellationToken);

                if (certificate is null)
                {
                    return NotFound(new
                    {
                        message = "Сертификат не найден."
                    });
                }

                var pdfBytes = await qualityCertificateService
                    .ExportCertificateToPdfAsync(id, cancellationToken);

                var fileName = $"{certificate.CertificateNumber}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
