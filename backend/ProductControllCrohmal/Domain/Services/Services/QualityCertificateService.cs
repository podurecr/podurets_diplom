using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;
using System.Text;

namespace Domain.Services.Services
{
    public class QualityCertificateService : IQualityCertificateService
    {
        private readonly IQualityCertificateRepository qualityCertificateRepository;
        private readonly IBatchRepository batchRepository;
        private readonly IUserRepository userRepository;
        private readonly IQualityAssessmentRepository qualityAssessmentRepository;
        private readonly IAnalysisResultRepository analysisResultRepository;
        private readonly IProductQualitySpecificationRepository productQualitySpecificationRepository;
        private readonly IProductRepository productRepository;

        public QualityCertificateService(
            IQualityCertificateRepository qualityCertificateRepository,
            IBatchRepository batchRepository,
            IUserRepository userRepository,
            IQualityAssessmentRepository qualityAssessmentRepository,
            IAnalysisResultRepository analysisResultRepository,
            IProductQualitySpecificationRepository productQualitySpecificationRepository, 
            IProductRepository productRepository)
        {
            this.qualityCertificateRepository = qualityCertificateRepository;
            this.batchRepository = batchRepository;
            this.userRepository = userRepository;
            this.qualityAssessmentRepository = qualityAssessmentRepository;
            this.analysisResultRepository = analysisResultRepository;
            this.productQualitySpecificationRepository = productQualitySpecificationRepository;
            this.productRepository = productRepository;
        }

        public async Task<List<QualityCertificateDTO>> GetCertificatesAsync(
            CancellationToken cancellationToken = default)
        {
            var certificates = await qualityCertificateRepository
                .GetAllWithDetailsAsync(cancellationToken);

            var result = new List<QualityCertificateDTO>();

            foreach (var cert in certificates) { 
                var lokal = EntityToDTOMapper.ToQualityCertificateDTO(cert);
                lokal.Batch = EntityToDTOMapper.ToBatchDTO(batchRepository.GetByIdAsync(cert.BatchId).Result);
                lokal.Batch.Product = EntityToDTOMapper.ToProductDTO(productRepository.GetByIdAsync(lokal.Batch.ProductId).Result);
                lokal.CreatedByUser = EntityToDTOMapper.ToUserDTO(userRepository.GetByIdAsync(cert.CreatedByUserId).Result);
                result.Add(lokal);
            }

            return result;
        }

        public async Task<QualityCertificateDTO?> GetCertificateByIdAsync(
            int certificateId,
            CancellationToken cancellationToken = default)
        {
            var certificate = await qualityCertificateRepository
                .GetByIdWithDetailsAsync(certificateId, cancellationToken);

            if (certificate is null)
                return null;

            return EntityToDTOMapper.ToQualityCertificateDTO(certificate);
        }

        public async Task<QualityCertificateDTO?> GetCertificateByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            var certificate = await qualityCertificateRepository
                .GetByBatchIdNoTrackingAsync(batchId, cancellationToken);

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

            var existingCertificate = await qualityCertificateRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (existingCertificate is not null)
                return EntityToDTOMapper.ToQualityCertificateDTO(existingCertificate);

            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);

            if (batch is null)
                throw new InvalidOperationException("Партия не найдена.");

            if (!batch.IsAnalysisCompleted)
                throw new InvalidOperationException("Нельзя сформировать сертификат, пока анализы не завершены.");

            if (batch.Status != BatchStatus.Approved)
                throw new InvalidOperationException("Сертификат можно сформировать только для пригодной партии.");

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                throw new InvalidOperationException("Пользователь не найден.");

            var assessment = await qualityAssessmentRepository
                .GetByBatchIdNoTrackingAsync(batchId, cancellationToken);

            if (assessment is null)
                throw new InvalidOperationException("Оценка качества для партии не найдена.");

            if (!assessment.IsFinal)
                throw new InvalidOperationException("Оценка качества должна быть финализирована.");

            if (!assessment.IsApproved)
                throw new InvalidOperationException("Нельзя сформировать сертификат для забракованной партии.");

            var results = await analysisResultRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (results.Count == 0)
                throw new InvalidOperationException("Для партии нет результатов анализов.");

            var certificate = new QualityCertificate
            {
                CertificateNumber = GenerateCertificateNumber(batch.Id),
                BatchId = batchId,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
                Conclusion = assessment.Conclusion,
                PdfPath = null
            };

            await qualityCertificateRepository.AddAsync(certificate, cancellationToken);
            await qualityCertificateRepository.SaveChangesAsync(cancellationToken);

            var createdCertificate = await qualityCertificateRepository
                .GetByBatchIdNoTrackingAsync(batchId, cancellationToken);

            if (createdCertificate is null)
                throw new InvalidOperationException("Сертификат был создан, но не найден после сохранения.");

            return EntityToDTOMapper.ToQualityCertificateDTO(createdCertificate);
        }

        public async Task<byte[]> ExportCertificateToPdfAsync(
            int certificateId,
            CancellationToken cancellationToken = default)
        {
            if (certificateId <= 0)
                throw new ArgumentException("Некорректный ID сертификата.");

            var certificate = await qualityCertificateRepository
                .GetByIdWithDetailsAsync(certificateId, cancellationToken);

            if (certificate is null)
                throw new InvalidOperationException("Сертификат не найден.");

            if (certificate.Batch is null)
                throw new InvalidOperationException("Партия сертификата не найдена.");

            var results = await analysisResultRepository
                .GetByBatchIdAsync(certificate.BatchId, cancellationToken);

            var specifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(certificate.Batch.ProductId, cancellationToken);

            return QualityCertificatePdfGenerator.Generate(
                certificate,
                results,
                specifications);
        }

        private static string GenerateCertificateNumber(int batchId)
        {
            return $"COA-{DateTime.UtcNow:yyyy}-{batchId:D5}";
        }
    }
}
