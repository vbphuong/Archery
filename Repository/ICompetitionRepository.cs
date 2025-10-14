using Archery.Models.DTO;
using Archery.Models.Entity;

namespace Archery.Models.Repository
{
    public interface ICompetitionRepository
    {
        Task<IEnumerable<Competition>> GetAllAsync();
        Task<Competition?> GetByIdAsync(int id);
        Task<Competition> CreateAsync(CompetitionDTO dto);
        Task<Competition?> UpdateAsync(int id, CompetitionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}