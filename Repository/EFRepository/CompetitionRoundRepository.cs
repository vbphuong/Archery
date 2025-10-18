using Archery.Data;
using Archery.Models.DTO;
using Archery.Models.Entity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector; // this library help us to write directly SQL in Repository

namespace Archery.Repository
{
    public class CompetitionRoundRepository : BaseRepository, ICompetitionRoundRepository
    {
        public CompetitionRoundRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CompetitionRoundDTO>> GetByCompetitionAsync(int competitionId)
        {
            var data = await (
                from cr in _context.CompetitionRounds
                join r in _context.Rounds on cr.RoundID equals r.RoundID
                where cr.CompetitionID == competitionId
                select new CompetitionRoundDTO
                {
                    CompetitionID = cr.CompetitionID,
                    RoundID = r.RoundID,
                    RoundName = r.Name,
                    Class = r.Class,
                    Gender = r.Gender,
                    EquipmentName = r.Equipment!.Type,
                    HasEquivalent = cr.HasEquivalent ?? "No",
                    IsPractice = cr.IsPractice ?? "No",
                    Status = cr.Status ?? "NotStarted",
                    DateCreated = cr.DateCreated,
                    TimeCreated = cr.TimeCreated
                }
            ).ToListAsync();

            return data;
        }


        public async Task<CompetitionRoundDTO?> AddAsync(CompetitionRoundDTO dto)
        {
            var entity = new CompetitionRound
            {
                RoundID = dto.RoundID,
                CompetitionID = dto.CompetitionID,
                HasEquivalent = dto.HasEquivalent ?? "No",
                IsPractice = dto.IsPractice ?? "No",
                Status = dto.Status ?? "NotStarted",
                TimeCreated = DateTime.Now,
                DateCreated = DateTime.Now
            };

            _context.CompetitionRounds.Add(entity);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<CompetitionRoundDTO?> UpdateAsync(int competitionId, int roundId, CompetitionRoundDTO dto)
        {
            var existing = await _context.CompetitionRounds
                .FirstOrDefaultAsync(x => x.CompetitionID == competitionId && x.RoundID == roundId);

            if (existing == null)
                return null;

            existing.HasEquivalent = dto.HasEquivalent ?? existing.HasEquivalent;
            existing.IsPractice = dto.IsPractice ?? existing.IsPractice;
            existing.Status = dto.Status ?? existing.Status;
            existing.TimeCreated = DateTime.Now;
            existing.DateCreated = DateTime.Now;

            await _context.SaveChangesAsync();

            return new CompetitionRoundDTO
            {
                RoundID = existing.RoundID,
                CompetitionID = existing.CompetitionID,
                HasEquivalent = existing.HasEquivalent,
                IsPractice = existing.IsPractice,
                Status = existing.Status,
                TimeCreated = existing.TimeCreated,
                DateCreated = existing.DateCreated
            };
        }

        public async Task<bool> DeleteAsync(int roundId, int competitionId)
        {
            var existing = await _context.CompetitionRounds
                .FirstOrDefaultAsync(x => x.RoundID == roundId && x.CompetitionID == competitionId);

            if (existing == null) return false;

            _context.CompetitionRounds.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RoundOptionDTO>> GetAvailableRoundsAsync()
        {
            var query =
                from r in _context.Rounds
                join eq in _context.EquivalentRounds on r.RoundID equals eq.BaseRoundID into eqj
                from eq in eqj.DefaultIfEmpty()
                join er in _context.Rounds on eq.EquivalentRoundID equals er.RoundID into erj
                from er in erj.DefaultIfEmpty()
                select new RoundOptionDTO
                {
                    RoundID = r.RoundID,
                    DisplayName = er != null
                        ? $"{r.Name} - Equivalent Available ({er.Name})"
                        : r.Name
                };

            return await query.ToListAsync();
        }
    }
}