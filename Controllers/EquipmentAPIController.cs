using Archery.Data;
using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EquipmentAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EquipmentAPIController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Equipments.ToList());
    }
}