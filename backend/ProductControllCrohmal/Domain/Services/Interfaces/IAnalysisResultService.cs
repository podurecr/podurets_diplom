using Domain.DTOs;

namespace Domain.Services.Interfaces
{
    public interface IAnalysisResultService
    {
        Task<List<AnalysisResultDTO>> GetAnalysisResultsAsync(
            CancellationToken cancellationToken = default);

        Task<List<AnalysisResultDTO>> GetResultsByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default);

        Task AddResultsAsync(
            int batchId,
            AnalysisResultDTO dto,
            int userId,
            CancellationToken cancellationToken = default);

        Task UpdateResultAsync(
            int resultId,
            AnalysisResultDTO dto,
            CancellationToken cancellationToken = default);

        Task CompleteBatchAnalysisAsync(
            int batchId,
            SaveBatchAnalysisRequestDTO dto,
            int userId,
            CancellationToken cancellationToken = default);

        Task FinishBatchAnalysisAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default);
    }
}
