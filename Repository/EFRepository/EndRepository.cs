using Archery.Data;
using Archery.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Archery.Repository
{
    public class EndRepository : BaseRepository, IEndRepository
    {
        public EndRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<End>> GetByRangeAsync(int rangeId)
        {
            return await _context.Ends
                .Include(e => e.Arrows)
                .Where(e => e.RangeID == rangeId)
                .ToListAsync();
        }
    }

}

