using Archery.Models.DTO;
using Archery.Repository;
using Archery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Archery.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EndAPIController : ControllerBase
    {
        private readonly IEndRepository _repo;
        private readonly AppDbContext _context;

        public EndAPIController(IEndRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        [HttpGet("{rangeId}")]
        public async Task<IActionResult> GetByRange(int rangeId)
        {
            var ends = await _context.Ends
                .Where(e => e.RangeID == rangeId)
                .Include(e => e.Arrows)
                .ToListAsync();

            if (!ends.Any())
                return NotFound(new { message = "No ends found for this range." });

            var result = ends.Select(e => new EndDTO
            {
                EndID = e.EndID,
                EndNumber = e.EndNumber,
                Arrows = e.Arrows.Select(a => new ArrowDTO
                {
                    ArrowID = a.ArrowID,
                    Value = a.Value
                }).ToList()
            }).ToList();

            return Ok(result);
        }
    }
}