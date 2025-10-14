using Archery.Models.DTO;
using Archery.Models.Entity;

namespace Archery.Repository
{
    public interface IPeopleRepository
    {
        Task<List<UserDTO>> GetAllAsync();
        Task<UserDTO?> GetByIdAsync(int id);
        Task<UserDTO> CreateAsync(UserDTO dto);
        Task<bool> UpdateAsync(int id, UserDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}