using Archery.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Archery.Repository
{
    public interface IArcherRepository
    {
        Task<List<ArcherDTO>> GetAllAsync();
        Task<ArcherDTO?> GetByIdAsync(int archerId);
        Task<ArcherDTO?> GetByUserIdAsync(int userId);
        Task<ArcherDTO> CreateAsync(ArcherDTO dto);
        Task<bool> UpdateAsync(int archerId, ArcherDTO dto);
        Task<bool> DeleteAsync(int archerId);
        Task<IEnumerable<EliteArcherDTO>> GetTopEliteArchersAsync(int topCount);
    }
}