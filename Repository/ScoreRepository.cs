using Archery.Data;
using Archery.Models.DTO;
using Archery.Models.Entity;
using Archery.Models.Factory;
using Microsoft.EntityFrameworkCore;

namespace Archery.Models.Repository
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly AppDbContext _context;

        public ScoreRepository(AppDbContext context)
        {
            _context = context;
        }

        // ADD Score (Factory-based)
        public async Task<ScoreResultDTO?> AddAsync(ScoreCreateDTO dto)
        {
            var exists = await _context.Scores.AnyAsync(s =>
                s.RoundID == dto.RoundID &&
                s.CompetitionID == dto.CompetitionID &&
                s.ArcherID == dto.ArcherID);

            if (exists)
            {
                // return null hoặc throw lỗi tùy nhu cầu
                throw new InvalidOperationException("Score for this Archer in this Round & Competition already exists.");
            }

            var compRound = await _context.CompetitionRounds
                .Include(cr => cr.Round)
                .FirstOrDefaultAsync(cr => cr.RoundID == dto.RoundID && cr.CompetitionID == dto.CompetitionID);

            if (compRound == null) return null;

            var archer = await _context.Archers
                .Include(a => a.ArcherEquipments)
                .ThenInclude(ae => ae.Equipment)
                .FirstOrDefaultAsync(a => a.ArcherID == dto.ArcherID);

            if (archer == null) return null;

            var round = compRound.Round;
            if (round == null) return null;

            bool sameEquipment = archer.ArcherEquipments.Any(ae => ae.EquipmentID == round.EquipmentID);
            bool sameGender = archer.Gender == round.Gender;
            bool hasEquivalent = compRound.HasEquivalent?.Equals("Yes", StringComparison.OrdinalIgnoreCase) ?? false;

            if (!hasEquivalent && !(sameEquipment && sameGender))
                return null;

            if (hasEquivalent && !sameEquipment)
                return null;

            var factory = ScoreFactoryProducer.GetFactory(hasEquivalent);
            var score = factory.CreateScore(dto.ArcherID, dto.RoundID, dto.CompetitionID);

            _context.Scores.Add(score);
            await _context.SaveChangesAsync();

            // Trả DTO gọn, không có navigation
            return new ScoreResultDTO
            {
                ArcherID = score.ArcherID,
                RoundID = score.RoundID,
                CompetitionID = score.CompetitionID,
                TotalScore = score.TotalScore,
                DateRecorded = score.DateRecorded,
                TimeRecorded = score.TimeRecorded
            };
        }


        // GET Scores by CompetitionRound
        public async Task<IEnumerable<Score>> GetByCompetitionRoundAsync(int competitionId, int roundId)
        {
            return await _context.Scores
                .Include(s => s.Archer)
                    .ThenInclude(a => a.User)
                .Include(s => s.Archer)
                    .ThenInclude(a => a.ArcherEquipments)
                        .ThenInclude(ae => ae.Equipment)
                .Where(s => s.CompetitionID == competitionId && s.RoundID == roundId)
                .ToListAsync();
        }

        // CHECK if Score exists
        public async Task<bool> ExistsAsync(int competitionId, int roundId, int archerId)
        {
            return await _context.Scores.AnyAsync(s =>
                s.CompetitionID == competitionId &&
                s.RoundID == roundId &&
                s.ArcherID == archerId);
        }


    }
}
