using Archery.Data;
using Archery.Models.DTO;
using Archery.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Archery.Repository
{
    public class CompetitionRepository : BaseRepository, ICompetitionRepository
    {
        public CompetitionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Competition>> GetAllAsync()
        {
            return await _context.Competitions
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<Competition?> GetByIdAsync(int id)
        {
            return await _context.Competitions.FindAsync(id);
        }

        public async Task<Competition> CreateAsync(CompetitionDTO dto)
        {
            var competition = new Competition
            {
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                IsChampionship = dto.IsChampionship,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Competitions.Add(competition);
            await _context.SaveChangesAsync();
            return competition;
        }

        public async Task<Competition?> UpdateAsync(int id, CompetitionDTO dto)
        {
            var existing = await _context.Competitions.FindAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.Date = dto.Date;
            existing.IsChampionship = dto.IsChampionship;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Competitions.FindAsync(id);
            if (existing == null) return false;

            _context.Competitions.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}