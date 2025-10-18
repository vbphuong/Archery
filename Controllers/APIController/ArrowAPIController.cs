using Microsoft.AspNetCore.Mvc;
using Archery.Models.DTO;
using Archery.Repository;
using System.Text.Json;

namespace Archery.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArrowAPIController : ControllerBase
    {
        private readonly IArrowRepository _repo;

        public ArrowAPIController(IArrowRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArrowDTO>>> GetAll()
        {
            var result = await _repo.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArrowDTO>> GetById(int id)
        {
            var arrow = await _repo.GetByIdAsync(id);
            if (arrow == null)
                return NotFound();

            return Ok(arrow);
        }

        [HttpGet("getbyend/{endId}")]
        public async Task<ActionResult<IEnumerable<ArrowDTO>>> GetByEnd(int endId)
        {
            var arrows = await _repo.GetByEndAsync(endId);
            return Ok(arrows);
        }

        [HttpPost]
        public async Task<ActionResult<ArrowDTO>> Add([FromBody] ArrowDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result!.ArrowID }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] JsonElement body)
        {
            if (!body.TryGetProperty("value", out var val))
                return BadRequest("Missing 'value' field");

            string? value = val.ValueKind switch
            {
                JsonValueKind.Number => val.GetInt32().ToString(),
                JsonValueKind.String => val.GetString(),
                JsonValueKind.Null => null,
                _ => null
            };

            var dto = new ArrowDTO { Value = value };

            var ok = await _repo.UpdateAsync(id, dto);
            if (!ok) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}
