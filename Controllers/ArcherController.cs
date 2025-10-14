using Archery.Models.DTO;
using Archery.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    [Controller]
    public class ArcherController : Controller
    {
        private readonly IArcherRepository _repo;

        public ArcherController(IArcherRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("Details/{userId}")]
        public async Task<IActionResult> Details(int userId)
        {
            Console.WriteLine($"Hit ArcherController.Details(userId={userId})");

            var archer = await _repo.GetByUserIdAsync(userId);

            // Nếu chưa có, tạo mới 1 object trống (để View hiển thị form)
            if (archer == null)
            {
                archer = new ArcherDTO
                {
                    UserId = userId
                };
                Console.WriteLine("Archer not found — returning empty model for creation");
            }

            return View(archer);
        }

    }
}
