using Archery.Models.DTO;
using Archery.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompetitionAPIController : ControllerBase
    {
        private readonly ICompetitionRepository _repo;

        public CompetitionAPIController(ICompetitionRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _repo.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var comp = await _repo.GetByIdAsync(id);
            if (comp == null) return NotFound();
            return Ok(comp);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<IActionResult> Create([FromBody] CompetitionDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required.");

            var created = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CompetitionID }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<IActionResult> Update(int id, [FromBody] CompetitionDTO dto)
        {
            var updated = await _repo.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok) return NotFound();
            return Ok();
        }
    }
}