using Archery.Models.DTO;
using Archery.Models.Entity;

namespace Archery.Repository
{
    public interface IScoreRepository
    {
        Task<ScoreResultDTO?> AddAsync(ScoreCreateDTO dto);
        Task<IEnumerable<Score>> GetByCompetitionRoundAsync(int competitionId, int roundId);
        Task<bool> ExistsAsync(int competitionId, int roundId, int archerId);
    }
}