using Archery.Data;
using Archery.Models.DTO;
using Archery.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Archery.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Recorder")]
    public class ScoreAPIController : ControllerBase
    {
        private readonly IScoreRepository _scoreRepo;
        private readonly AppDbContext _context;

        public ScoreAPIController(IScoreRepository scoreRepo, AppDbContext context)
        {
            _scoreRepo = scoreRepo;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ScoreCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid input.");

            if (await _scoreRepo.ExistsAsync(dto.CompetitionID, dto.RoundID, dto.ArcherID))
                return Conflict("This archer already has a score for this round.");

            var result = await _scoreRepo.AddAsync(dto);
            if (result == null)
                return BadRequest("Unable to create Score record.");

            return Ok(new
            {
                result.CompetitionID,
                result.RoundID,
                result.ArcherID,
                result.TotalScore
            });
        }

        [HttpGet("{competitionId}/{roundId}")]
        public async Task<IActionResult> GetByRound(int competitionId, int roundId)
        {
            var list = await _scoreRepo.GetByCompetitionRoundAsync(competitionId, roundId);
            return Ok(list.Select(s => new
            {
                s.CompetitionID,
                s.RoundID,
                s.ArcherID,
                ArcherName = s.Archer.User!.Email,
                s.TotalScore,
                s.ApprovalStatus
            }));
        }

        // GET: api/scoreapi/gettotal/{competitionId}/{roundId}/{archerId}
        [HttpGet("gettotal/{competitionId}/{roundId}/{archerId}")]
        public async Task<ActionResult<int>> GetTotalScore(int competitionId, int roundId, int archerId)
        {
            // Lấy tất cả các Range thuộc archer này trong round & competition
            var ranges = await _context.ArcheryRanges
                .Where(r => r.CompetitionID == competitionId && r.RoundID == roundId && r.ArcherID == archerId)
                .Include(r => r.Ends)
                    .ThenInclude(e => e.Arrows)
                .ToListAsync();

            if (!ranges.Any())
                return Ok(0);

            int total = 0;

            foreach (var range in ranges)
            {
                foreach (var end in range.Ends)
                {
                    foreach (var arrow in end.Arrows)
                    {
                        if (int.TryParse(arrow.Value, out int val))
                            total += val;
                    }
                }
            }

            // Tự động lưu vào bảng Score (nếu có)
            var score = await _context.Scores.FirstOrDefaultAsync(s =>
                s.CompetitionID == competitionId &&
                s.RoundID == roundId &&
                s.ArcherID == archerId);

            if (score == null)
            {
                score = new Models.Entity.Score
                {
                    CompetitionID = competitionId,
                    RoundID = roundId,
                    ArcherID = archerId,
                    TotalScore = total,
                    DateRecorded = DateTime.UtcNow,
                    ApprovalStatus = "Pending"
                };
                _context.Scores.Add(score);
            }
            else
            {
                score.TotalScore = total;
                score.DateRecorded = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return Ok(new { totalScore = total });

        }
    }
}