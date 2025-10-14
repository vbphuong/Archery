using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Archery.Data;
using Archery.Hubs;
using Archery.Models.Entity;
using Archery.Models.DTO;

namespace Archery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InboxAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<InboxHub> _hubContext;

        public InboxAPIController(AppDbContext db, IHubContext<InboxHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send([FromBody] InboxSendDto dto)
        {
            try
            {
                var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var model = new Inbox
                {
                    Sender = senderId,
                    Receiver = dto.Receiver,
                    Message = dto.Message,
                    SentAt = DateTime.UtcNow,
                    IsRead = false,
                    Status = "sent"
                };

                _db.Inbox.Add(model);
                await _db.SaveChangesAsync();

                // Push qua SignalR cho Receiver
                await _hubContext.Clients.User(model.Receiver.ToString())
                    .SendAsync("ReceiveMessage", senderId, model.Receiver, model.Message, model.SentAt);

                // Push luôn cho Sender để hiển thị ngay
                await _hubContext.Clients.User(senderId.ToString())
                    .SendAsync("ReceiveMessage", senderId, model.Receiver, model.Message, model.SentAt);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }



        [HttpGet("History/{userId}")]
        public async Task<IActionResult> History(int userId)
        {
            var myId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var messages = await _db.Inbox
                .Where(x => (x.Sender == myId && x.Receiver == userId) ||
                            (x.Sender == userId && x.Receiver == myId))
                .OrderBy(x => x.SentAt)
                .ToListAsync();

            return Ok(messages); 
        }
    }
}
