using Archery.Models.DTO;
using Archery.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompetitionRoundAPIController : ControllerBase
    {
        private readonly ICompetitionRoundRepository _repo;
        private readonly IScoreRepository _scoreRepo;

        public CompetitionRoundAPIController(
            ICompetitionRoundRepository repo,
            IScoreRepository scoreRepo)
        {
            _repo = repo;
            _scoreRepo = scoreRepo;
        }

        [HttpGet("{competitionId}")]
        public async Task<IActionResult> GetByCompetition(int competitionId)
        {
            try
            {
                var result = await _repo.GetByCompetitionAsync(competitionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetByCompetition: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableRounds()
            => Ok(await _repo.GetAvailableRoundsAsync());

        [HttpPost]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<IActionResult> Add([FromBody] CompetitionRoundDTO dto)
        {
            var result = await _repo.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut("{competitionId}/{roundId}")]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<IActionResult> Update(int competitionId, int roundId, [FromBody] CompetitionRoundDTO dto)
        {
            var updated = await _repo.UpdateAsync(competitionId, roundId, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }


        [HttpDelete("{competitionId}/{roundId}")]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<IActionResult> Delete(int competitionId, int roundId)
        {
            var ok = await _repo.DeleteAsync(roundId, competitionId);
            if (!ok) return NotFound();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AssignArcherToRound([FromBody] AssignArcherDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid payload.");

            var scoreDto = new ScoreCreateDTO
            {
                ArcherID = dto.ArcherID,
                RoundID = dto.RoundID,
                CompetitionID = dto.CompetitionID
            };

            var result = await _scoreRepo.AddAsync(scoreDto);
            if (result == null)
                return BadRequest("❌ Archer not eligible for this round.");

            return Ok(new { message = "✅ Archer assigned successfully.", result });
        }

        [HttpGet("{competitionId}/{roundId}/Archers")]
        public async Task<IActionResult> GetArchersForRound(int competitionId, int roundId)
        {
            try
            {
                var scores = await _scoreRepo.GetByCompetitionRoundAsync(competitionId, roundId);
                if (scores == null || !scores.Any())
                    return Ok(new List<object>());

                var result = scores.Select(s => new {
                    s.Archer.ArcherID,
                    FullName = $"{s.Archer.User!.FirstName} {s.Archer.User.LastName}",
                    s.Archer.Gender,
                    Equipments = s.Archer.ArcherEquipments.Select(ae => ae.Equipment.Type)
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetArchersForRound: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

    }
}