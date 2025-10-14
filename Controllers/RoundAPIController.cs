using Archery.Repository;
using Archery.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoundAPIController : ControllerBase
    {
        private readonly IRoundRepository _repo;

        public RoundAPIController(IRoundRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetAllRounds")]
        public async Task<IActionResult> GetAllRounds(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _repo.GetAllRoundsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost("AddEquivalent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEquivalentByName([FromBody] EquivalentRoundCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.EquivalentRoundName))
                return BadRequest("Invalid data");

            try
            {
                await _repo.AddEquivalentByNameAsync(dto.BaseRoundId, dto.EquivalentRoundName);
                return Ok(new { message = "Equivalent round added successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetAllRoundNames")]
        public async Task<IActionResult> GetAllRoundNames()
        {
            var rounds = await _repo.GetAllRoundNamesAsync();
            return Ok(rounds);
        }
    }
}