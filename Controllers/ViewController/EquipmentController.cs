using Microsoft.AspNetCore.Mvc;
using Archery.Data;

namespace Archery.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly AppDbContext _context;

        public EquipmentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var equipments = _context.Equipments.ToList();
            return View(equipments);
        }
    }
}