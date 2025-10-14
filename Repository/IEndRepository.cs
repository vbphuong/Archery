using Archery.Models.Entity;

namespace Archery.Repository
{
    public interface IEndRepository
    {
        Task<IEnumerable<End>> GetByRangeAsync(int rangeId);
    }

}

