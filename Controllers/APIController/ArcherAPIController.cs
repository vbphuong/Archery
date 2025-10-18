using Archery.Models.DTO;
using Archery.Repository;
using Archery.Services;
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
        private readonly EliteInMemoryService _eliteService;

        public ArcherAPIController(IArcherRepository repo, EliteInMemoryService eliteService)
        {
            _repo = repo;
            _eliteService = eliteService;
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

        // GET: api/ArcherAPI/EliteArchers
        [HttpGet("EliteArchers")]
        public async Task<IActionResult> GetEliteArchers()
        {
            var elites = (await _repo.GetTopEliteArchersAsync(10)).ToList();

            // Attach a canonical gift list (same for all elites). You can change it or read from config.
            var gifts = GetCanonicalGiftList();

            foreach (var e in elites)
            {
                e.Gifts = gifts.Select((g, idx) => new GiftDTO { Id = idx, GiftName = g.GiftName, Description = g.Description }).ToList();

                // nếu đã chọn quà trong memory service, attach
                if (_eliteService.TryGetChoice(e.MonthYear, e.ArcherId, out var chosen))
                {
                    e.SelectedGift = chosen;
                }
            }

            return Ok(elites);
        }

        // POST: api/ArcherAPI/EliteChoice
        // body: { "archerId": 12, "monthYear": "2025-10", "giftId": 2, "giftName": "Arrow Set" }
        [HttpPost("EliteChoice")]
        public IActionResult SaveEliteChoice([FromBody] EliteChoiceRequest req)
        {
            if (req == null || req.ArcherId <= 0 || string.IsNullOrEmpty(req.MonthYear) || string.IsNullOrEmpty(req.GiftName))
                return BadRequest("Invalid payload.");

            // Save in-memory (overwrite allowed)
            _eliteService.SaveChoice(req.MonthYear, req.ArcherId, req.GiftName);

            return Ok(new { success = true });
        }

        private List<GiftDTO> GetCanonicalGiftList()
        {
            return new List<GiftDTO>
            {
                new GiftDTO { Id = 0, GiftName = "Premium Bow", Description = "High-end competition bow" },
                new GiftDTO { Id = 1, GiftName = "Arrow Set", Description = "12 professional arrows" },
                new GiftDTO { Id = 2, GiftName = "Coaching Voucher", Description = "Free 1-hour coaching session" },
                new GiftDTO { Id = 3, GiftName = "Target Board", Description = "High-quality target board" },
                new GiftDTO { Id = 4, GiftName = "Grip Tape", Description = "Custom grip tape" },
                new GiftDTO { Id = 5, GiftName = "Quiver", Description = "Stylish leather quiver" },
                new GiftDTO { Id = 6, GiftName = "Arm Guard", Description = "Protective arm guard" },
                new GiftDTO { Id = 7, GiftName = "Shooting Glove", Description = "Professional glove" },
                new GiftDTO { Id = 8, GiftName = "Bowstring Wax", Description = "Bowstring maintenance kit" },
                new GiftDTO { Id = 9, GiftName = "Training Mat", Description = "Comfort mat for practice" }
            };
        }

        public class EliteChoiceRequest
        {
            public int ArcherId { get; set; }
            public string MonthYear { get; set; } = string.Empty;
            public int GiftId { get; set; }
            public string GiftName { get; set; } = string.Empty;
        }
    }
}
