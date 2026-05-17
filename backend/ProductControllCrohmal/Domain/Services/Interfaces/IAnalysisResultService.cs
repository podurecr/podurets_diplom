using Domain.DTOs;

namespace Domain.Services.Interfaces
{
    public interface IAnalysisResultService
    {
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
    }
}
