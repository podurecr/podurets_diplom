using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Repositories;
using System.Text;

namespace Domain.Services.Services
{
    public class QualityCertificateService : IQualityCertificateService
    {
        private readonly QualityCertificateRepository qualityCertificateRepository;
        private readonly BatchRepository batchRepository;
        private readonly UserRepository userRepository;
        private readonly AnalysisResultRepository analysisResultRepository;
        private readonly QualityAssessmentRepository qualityAssessmentRepository;

        public QualityCertificateService(
            QualityCertificateRepository qualityCertificateRepository,
            BatchRepository batchRepository,
            UserRepository userRepository,
            AnalysisResultRepository analysisResultRepository,
            QualityAssessmentRepository qualityAssessmentRepository)
        {
            this.qualityCertificateRepository = qualityCertificateRepository;
            this.batchRepository = batchRepository;
            this.userRepository = userRepository;
            this.analysisResultRepository = analysisResultRepository;
            this.qualityAssessmentRepository = qualityAssessmentRepository;
        }

        public async Task<QualityCertificateDTO?> GetCertificateByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            if (batchId <= 0)
                throw new ArgumentException("Некорректный ID партии.");

            var certificate = await qualityCertificateRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (certificate is null)
                return null;

            return EntityToDTOMapper.ToQualityCertificateDTO(certificate);
        }

        public async Task<QualityCertificateDTO> GenerateCertificateAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (batchId <= 0)
                throw new ArgumentException("Некорректный ID партии.");

            if (userId <= 0)
                throw new ArgumentException("Некорректный ID пользователя.");

            var batch = await batchRepository.GetWithDetailsAsync(batchId, cancellationToken);

            if (batch is null)
                throw new InvalidOperationException("Партия не найдена.");

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                throw new InvalidOperationException("Пользователь не найден.");

            if (batch.Status != BatchStatus.Approved)
                throw new InvalidOperationException("Сертификат можно сформировать только для одобренной партии.");

            var analysisResults = await analysisResultRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (analysisResults.Count == 0)
                throw new InvalidOperationException("Нельзя сформировать сертификат без результатов анализов.");

            var assessment = await qualityAssessmentRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (assessment is null)
                throw new InvalidOperationException("Нельзя сформировать сертификат без оценки качества.");

            if (!assessment.IsApproved)
                throw new InvalidOperationException("Нельзя сформировать сертификат для партии, которая не прошла оценку качества.");

            var existingCertificate = await qualityCertificateRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (existingCertificate is not null)
                throw new InvalidOperationException("Для этой партии сертификат уже создан.");

            var certificate = new QualityCertificate
            {
                CertificateNumber = GenerateCertificateNumber(batch),
                BatchId = batchId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                Conclusion = assessment.Conclusion,
                PdfPath = null
            };

            await qualityCertificateRepository.AddAsync(certificate, cancellationToken);
            await qualityCertificateRepository.SaveChangesAsync(cancellationToken);

            certificate.Batch = batch;
            certificate.CreatedByUser = user;

            return EntityToDTOMapper.ToQualityCertificateDTO(certificate);
        }

        public async Task<byte[]> ExportCertificateToPdfAsync(
            int certificateId,
            CancellationToken cancellationToken = default)
        {
            if (certificateId <= 0)
                throw new ArgumentException("Некорректный ID сертификата.");

            var certificate = await qualityCertificateRepository
                .GetByIdAsync(certificateId, cancellationToken);

            if (certificate is null)
                throw new KeyNotFoundException("Сертификат не найден.");

            var batch = await batchRepository
                .GetWithDetailsAsync(certificate.BatchId, cancellationToken);

            var user = await userRepository
                .GetByIdAsync(certificate.CreatedByUserId, cancellationToken);

            certificate.Batch = batch;
            certificate.CreatedByUser = user;

            return BuildSimplePdf(certificate);
        }

        private static string GenerateCertificateNumber(Batch batch)
        {
            return $"COA-{DateTime.UtcNow:yyyyMMddHHmmss}-{batch.Id}";
        }

        private static byte[] BuildSimplePdf(QualityCertificate certificate)
        {
            var batch = certificate.Batch;

            var lines = new List<string>
            {
                "QUALITY CERTIFICATE",
                "",
                $"Certificate number: {certificate.CertificateNumber}",
                $"Created at: {certificate.CreatedAt:yyyy-MM-dd HH:mm}",
                $"Batch ID: {certificate.BatchId}",
                $"Batch number: {batch?.BatchNumber ?? "-"}",
                $"Product: {batch?.Product?.Name ?? "-"}",
                $"Quantity: {batch?.Quantity.ToString() ?? "-"} {batch?.Unit ?? string.Empty}",
                $"Production date: {batch?.ProductionDate:yyyy-MM-dd}",
                $"Created by user ID: {certificate.CreatedByUserId}",
                $"Created by: {certificate.CreatedByUser?.FullName ?? "-"}",
                "",
                "Conclusion:",
                certificate.Conclusion
            };

            return CreatePdfFromLines(lines);
        }

        private static byte[] CreatePdfFromLines(List<string> lines)
        {
            var contentBuilder = new StringBuilder();

            contentBuilder.AppendLine("BT");
            contentBuilder.AppendLine("/F1 12 Tf");
            contentBuilder.AppendLine("50 790 Td");

            for (var i = 0; i < lines.Count; i++)
            {
                if (i > 0)
                    contentBuilder.AppendLine("0 -18 Td");

                var text = PreparePdfText(lines[i]);
                contentBuilder.AppendLine($"({EscapePdfText(text)}) Tj");
            }

            contentBuilder.AppendLine("ET");

            var content = contentBuilder.ToString();
            var encoding = Encoding.ASCII;

            var pdf = new StringBuilder();
            var offsets = new List<int>();

            pdf.Append("%PDF-1.4\n");

            AddObject(pdf, offsets, 1, "<< /Type /Catalog /Pages 2 0 R >>");
            AddObject(pdf, offsets, 2, "<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
            AddObject(pdf, offsets, 3,
                "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R >> >> /Contents 5 0 R >>");
            AddObject(pdf, offsets, 4,
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

            var contentLength = encoding.GetByteCount(content);

            AddObject(pdf, offsets, 5,
                $"<< /Length {contentLength} >>\nstream\n{content}endstream");

            var xrefPosition = encoding.GetByteCount(pdf.ToString());

            pdf.Append("xref\n");
            pdf.Append("0 6\n");
            pdf.Append("0000000000 65535 f \n");

            foreach (var offset in offsets)
            {
                pdf.Append($"{offset:0000000000} 00000 n \n");
            }

            pdf.Append("trailer\n");
            pdf.Append("<< /Size 6 /Root 1 0 R >>\n");
            pdf.Append("startxref\n");
            pdf.Append($"{xrefPosition}\n");
            pdf.Append("%%EOF");

            return encoding.GetBytes(pdf.ToString());
        }

        private static void AddObject(
            StringBuilder pdf,
            List<int> offsets,
            int objectNumber,
            string body)
        {
            var encoding = Encoding.ASCII;

            offsets.Add(encoding.GetByteCount(pdf.ToString()));

            pdf.Append($"{objectNumber} 0 obj\n");
            pdf.Append(body);
            pdf.Append("\nendobj\n");
        }

        private static string EscapePdfText(string text)
        {
            return text
                .Replace("\\", "\\\\")
                .Replace("(", "\\(")
                .Replace(")", "\\)");
        }

        private static string PreparePdfText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "-";

            // Простая PDF-заглушка на Helvetica нормально работает с ASCII.
            // Для полноценной кириллицы позже лучше подключить QuestPDF или другой PDF-генератор со шрифтами.
            return new string(text.Select(ch => ch <= 127 ? ch : '?').ToArray());
        }
    }
}
