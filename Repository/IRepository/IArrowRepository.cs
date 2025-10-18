using Archery.Models.DTO;

namespace Archery.Repository
{
    public interface IArrowRepository
    {
        Task<IEnumerable<ArrowDTO>> GetAllAsync();
        Task<ArrowDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ArrowDTO>> GetByEndAsync(int endId);
        Task<ArrowDTO> CreateAsync(ArrowDTO dto);
        Task<bool> UpdateAsync(int id, ArrowDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
