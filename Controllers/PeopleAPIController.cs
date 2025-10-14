using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Archery.Repository;
using Archery.Models.DTO;

namespace Archery.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class PeopleAPIController : ControllerBase
    {
        private readonly IPeopleRepository _repo;

        public PeopleAPIController(IPeopleRepository repo)
        {
            _repo = repo;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            return Ok(await _repo.GetAllAsync());
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            var dto = await _repo.GetByIdAsync(id);
            if (dto == null) return NotFound("User not found");
            return Ok(dto);
        }

        
        [HttpPost]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<ActionResult<UserDTO>> Create([FromBody] UserDTO dto)
        {
            try
            {
                var created = await _repo.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<ActionResult> Update(int id, [FromBody] UserDTO dto)
        {
            var currentUserRole = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            var currentUserEmail = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            var targetUser = await _repo.GetByIdAsync(id);
            if (targetUser == null)
                return NotFound("User not found");

            if (currentUserRole == "Recorder" && targetUser.RoleName != "Archer")
                return Forbid("Recorders can only update Archers");

            if (currentUserRole == "Recorder" && targetUser.Email == currentUserEmail)
                return Forbid("You cannot modify yourself");

            var success = await _repo.UpdateAsync(id, dto);
            if (!success)
                return BadRequest("Failed to update user");

            return NoContent();
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recorder")]
        public async Task<ActionResult> Delete(int id)
        {
            var currentUserRole = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            var currentUserEmail = User.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            var targetUser = await _repo.GetByIdAsync(id);
            if (targetUser == null)
                return NotFound("User not found");

            if (currentUserRole == "Recorder" && targetUser.RoleName != "Archer")
                return Forbid("Recorders can only delete Archers");

            if (currentUserRole == "Recorder" && targetUser.Email == currentUserEmail)
                return Forbid("You cannot delete yourself");

            var success = await _repo.DeleteAsync(id);
            if (!success) return BadRequest("Failed to delete user");

            return NoContent();
        }

    }
}