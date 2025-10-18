using Archery.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Archery.Controllers
{
    //[Authorize]
    public class CompetitionRoundController : Controller
    {
        private readonly AppDbContext _context;

        public CompetitionRoundController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int competitionId, int roundId, int archerId)
        {
            var ranges = await _context.ArcheryRanges
                .Include(r => r.Ends)
                    .ThenInclude(e => e.Arrows)
                .Where(r => r.CompetitionID == competitionId &&
                            r.RoundID == roundId &&
                            r.ArcherID == archerId)
                .ToListAsync();


            if (!ranges.Any())
                return NotFound("No Archery Ranges found for this Archer in the selected Round.");

            ViewBag.CompetitionId = competitionId;
            ViewBag.RoundId = roundId;
            ViewBag.ArcherId = archerId;

            return View(ranges);
        }
    }
}
