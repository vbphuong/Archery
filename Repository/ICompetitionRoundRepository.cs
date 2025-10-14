using Archery.Models.DTO;

namespace Archery.Models.Repository
{
    public interface ICompetitionRoundRepository
    {
        Task<IEnumerable<CompetitionRoundDTO>> GetByCompetitionAsync(int competitionId);
        Task<CompetitionRoundDTO?> AddAsync(CompetitionRoundDTO dto);
        Task<CompetitionRoundDTO?> UpdateAsync(int competitionId, int roundId, CompetitionRoundDTO dto);
        Task<bool> DeleteAsync(int roundId, int competitionId);
        Task<IEnumerable<RoundOptionDTO>> GetAvailableRoundsAsync();
    }

    public class RoundOptionDTO
    {
        public int RoundID { get; set; }
        public string DisplayName { get; set; } = null!;
    }
}