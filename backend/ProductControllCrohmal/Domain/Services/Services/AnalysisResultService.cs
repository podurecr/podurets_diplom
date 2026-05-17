using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Services
{
    public class AnalysisResultService : IAnalysisResultService
    {
        private readonly AnalysisResultRepository analysisResultRepository;

        public AnalysisResultService(AnalysisResultRepository analysisResultRepository)
        {
            this.analysisResultRepository = analysisResultRepository;
        }

        public Task AddResultsAsync(int batchId, AnalysisResultDTO dto, int userId, CancellationToken cancellationToken = default)
        {
            return analysisResultRepository.AddAsync(DTOToEntityMapper.ToAnalysisResultEntity(batchId, dto, userId));
        }

        public async Task<List<AnalysisResultDTO>> GetResultsByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<AnalysisResult> analysisResults =
                await analysisResultRepository.GetByBatchIdAsync(batchId, cancellationToken);

            return analysisResults
                .Select(EntityToDTOMapper.ToAnalysisResultDTO)
                .ToList();
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
    }
}
