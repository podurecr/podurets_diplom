using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Services
{
    public class AnalysisResultService : IAnalysisResultService
    {
        private readonly IAnalysisResultRepository analysisResultRepository;
        private readonly IProductQualitySpecificationRepository productQualitySpecificationRepository;
        private readonly IBatchRepository batchRepository;
        private readonly IUserRepository userRepository;
        private readonly IQualityParameterRepository qualityParameterRepository;
        private readonly IProductRepository productRepository;

        public AnalysisResultService(IAnalysisResultRepository analysisResultRepository, IBatchRepository batchRepository, IUserRepository userRepository, IQualityParameterRepository qualityParameterRepository, IProductQualitySpecificationRepository productQualitySpecificationRepository, IProductRepository productRepository)
        {
            this.analysisResultRepository = analysisResultRepository;
            this.batchRepository = batchRepository;
            this.userRepository = userRepository;
            this.qualityParameterRepository = qualityParameterRepository;
            this.productQualitySpecificationRepository = productQualitySpecificationRepository;
            this.productRepository = productRepository;
        }

        public Task AddResultsAsync(int batchId, AnalysisResultDTO dto, int userId, CancellationToken cancellationToken = default)
        {
            return analysisResultRepository.AddAsync(DTOToEntityMapper.ToAnalysisResultEntity(batchId, dto, userId));
        }

        public async Task<List<AnalysisResultDTO>> GetAnalysisResultsAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<AnalysisResult> list =
                await analysisResultRepository.GetAllAsync(cancellationToken);

            List<AnalysisResultDTO> result = new List<AnalysisResultDTO>();

            foreach (AnalysisResult analysisResult in list)
            {
                AnalysisResultDTO dto = EntityToDTOMapper.ToAnalysisResultDTO(analysisResult);

                var enteredByUser = await userRepository.GetByIdAsync(
                    dto.EnteredByUserId,
                    cancellationToken);

                if (enteredByUser is not null)
                {
                    dto.EnteredByUser = EntityToDTOMapper.ToUserDTO(enteredByUser);
                }

                var batch = EntityToDTOMapper.ToBatchDTO(await batchRepository.GetByIdAsync(dto.BatchId, cancellationToken));

                batch.Product = EntityToDTOMapper.ToProductDTO(productRepository.GetByIdAsync(batch.ProductId).Result);

                var qualityParameter = EntityToDTOMapper.ToQualityParameterDTO(this.qualityParameterRepository.GetByIdAsync(dto.QualityParameterId).Result);

                dto.QualityParameter = qualityParameter;

                if (batch is not null)
                {
                    dto.Batch = batch;
                }

                result.Add(dto);
            }

            return result;
        }

        public async Task<List<AnalysisResultDTO>> GetResultsByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<AnalysisResult> analysisResults =
                await analysisResultRepository.GetByBatchIdAsync(batchId, cancellationToken);

            var list = new List<AnalysisResultDTO>();

            foreach(var analysisResult in analysisResults)
            {
                var dto = EntityToDTOMapper.ToAnalysisResultDTO(analysisResult);
                dto.QualityParameter = EntityToDTOMapper.ToQualityParameterDTO(qualityParameterRepository.GetByIdAsync(dto.QualityParameterId).Result);
                list.Add(dto);
            }

            return list;
        }

        public async Task UpdateResultAsync(
            int resultId,
            AnalysisResultDTO dto,
            CancellationToken cancellationToken = default)
        {
            var analysisResult = await analysisResultRepository.GetByIdAsync(resultId, cancellationToken);

            if (analysisResult is null)
                throw new KeyNotFoundException("Результат анализа не найден.");

            analysisResult.NumericValue = dto.NumericValue;
            analysisResult.TextValue = dto.TextValue;
            analysisResult.IsWithinNorm = dto.IsWithinNorm;
            analysisResult.Comment = dto.Comment;
            analysisResult.AnalyzedAt = DateTime.UtcNow;
            analysisResult.EnteredByUserId = dto.EnteredByUserId;
            analysisResult.QualityParameterId = dto.QualityParameterId;
            analysisResult.BatchId = dto.BatchId;

            analysisResultRepository.Update(analysisResult);

            await analysisResultRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task CompleteBatchAnalysisAsync(
             int batchId,
             SaveBatchAnalysisRequestDTO dto,
             int userId,
             CancellationToken cancellationToken = default)
        {
            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);


            var user = await userRepository.GetByIdAsync(userId, cancellationToken);


            var existingResults = await analysisResultRepository
                .GetByBatchIdForUpdateAsync(batchId, cancellationToken);

            foreach (var item in dto.Results)
            {
                var existingResult = existingResults.FirstOrDefault(x =>
                    x.QualityParameterId == item.QualityParameterId);

                if (existingResult is null)
                {
                    var result = new AnalysisResult
                    {
                        BatchId = batchId,
                        QualityParameterId = item.QualityParameterId,
                        NumericValue = item.NumericValue,
                        TextValue = item.TextValue,
                        IsWithinNorm = item.IsWithinNorm,
                        Comment = item.Comment,
                        EnteredByUserId = userId,
                        AnalyzedAt = DateTime.UtcNow
                    };

                    await analysisResultRepository.AddAsync(result, cancellationToken);
                }
                else
                {
                    existingResult.NumericValue = item.NumericValue;
                    existingResult.TextValue = item.TextValue;
                    existingResult.IsWithinNorm = item.IsWithinNorm;
                    existingResult.Comment = item.Comment;
                    existingResult.EnteredByUserId = userId;
                    existingResult.AnalyzedAt = DateTime.UtcNow;
                }
            }

            await analysisResultRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task FinishBatchAnalysisAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            var results = await analysisResultRepository.GetByBatchIdAsync(
                batchId,
                cancellationToken);

            batch.IsAnalysisCompleted = true;

            await batchRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
