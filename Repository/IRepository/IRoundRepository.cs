using Archery.Models.DTO;

namespace Archery.Repository
{
    public interface IRoundRepository
    {
        Task<PagedResult<RoundDTO>> GetAllRoundsAsync(int pageNumber, int pageSize);
        Task AddEquivalentAsync(int baseRoundId, int equivalentRoundId);
        Task<IEnumerable<RoundDTO>> GetAllRoundNamesAsync();
        Task AddEquivalentByNameAsync(int baseRoundId, string equivalentRoundName);
    }
}