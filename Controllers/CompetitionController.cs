using Archery.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    //[Authorize]
    public class CompetitionController : Controller
    {
        private readonly ICompetitionRepository _repo;
        private readonly ICompetitionRoundRepository _roundRepo;

        public CompetitionController(ICompetitionRepository competitionRepo, ICompetitionRoundRepository roundRepo)
        {
            _repo = competitionRepo;
            _roundRepo = roundRepo;
        }

        public async Task<IActionResult> Details(int id)
        {
            var comp = await _repo.GetByIdAsync(id);
            if (comp == null) return NotFound();

            var rounds = await _roundRepo.GetByCompetitionAsync(id);
            ViewBag.Competition = comp;
            return View(rounds);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var competitions = await _repo.GetAllAsync();
            return View(competitions);
        }
    }
}