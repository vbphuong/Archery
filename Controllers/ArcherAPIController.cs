using Archery.Models.DTO;
using Archery.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Archery.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArcherAPIController : ControllerBase
    {
        private readonly IArcherRepository _repo;

        public ArcherAPIController(IArcherRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("GetAll")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ArcherDTO>>> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ArcherDTO>> GetById(int id)
        {
            var archer = await _repo.GetByIdAsync(id);
            if (archer == null) return NotFound();
            return Ok(archer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArcherDTO dto)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var target = await _repo.GetByIdAsync(id);
            if (target == null) return NotFound();

            bool canEdit = role == "Admin" || role == "Recorder" || (role == "Archer" && target.UserId == currentUserId);
            if (!canEdit) return Forbid();

            dto.Role = role;

            var success = await _repo.UpdateAsync(id, dto);
            if (!success) return BadRequest("Failed to update");

            return NoContent();
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] ArcherDTO dto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Admin" && role != "Recorder" && currentUserId != dto.UserId)
                return Forbid();

            dto.Role = role;

            var existing = await _repo.GetByUserIdAsync(dto.UserId);
            if (existing == null)
                await _repo.CreateAsync(dto);
            else
                await _repo.UpdateAsync(existing.ArcherId, dto);

            return Ok(new { message = "Saved successfully" });
        }
    }
}
