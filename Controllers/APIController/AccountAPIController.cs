using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Archery.Services;
using Archery.Models.API;
using Archery.Models.Entity;
using Archery.Data;
using Archery.Handlers;
using Archery.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Archery.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountAPIController : ControllerBase
	{
		private readonly JwtService _jwtService;
		private readonly AppDbContext _dbContext;

		public AccountAPIController(JwtService jwtService, AppDbContext dbContext)
		{
			_jwtService = jwtService;
			_dbContext = dbContext;
		}

		[AllowAnonymous]
		[HttpPost("Login")]
		public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginRequestModel request)
		{
			var result = await _jwtService.Authenticate(request);
			if (result is null)
				return Unauthorized();

			return result;
		}

		[AllowAnonymous]
		[HttpPost("Register")]
		public async Task<ActionResult> Register([FromBody] User user)
		{
			if (string.IsNullOrWhiteSpace(user.Email) ||
				string.IsNullOrWhiteSpace(user.Password) ||
				string.IsNullOrWhiteSpace(user.FirstName) ||
				string.IsNullOrWhiteSpace(user.LastName))
			{
				return BadRequest("Please fill in all information");
			}

			var existingUser = await _dbContext.User.FirstOrDefaultAsync(x => x.Email == user.Email);
			if (existingUser != null)
			{
				return BadRequest("This user is already exist");
			}

			user.Password = PasswordHashHandler.HashPassword(user.Password);
			user.RoleID = 3;
			await _dbContext.User.AddAsync(user);
			await _dbContext.SaveChangesAsync();

			return Ok("Successful Registeration");
		}

        [HttpGet("SignOut")]
        public async Task<IActionResult> SignOutUser()
        {
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
            return Ok(new { redirectUrl = "/Account/Login" });
        }

        [HttpPost("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("New password and confirmation do not match.");
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Invalid token: no user identifier");

            var userId = int.Parse(userIdClaim.Value);


            var user = await _dbContext.User.FirstOrDefaultAsync(u => u.UserID == userId);
            if (user == null) return Unauthorized();

            if (!PasswordHashHandler.VerifyPassword(model.CurrentPassword, user.Password))
            {
                return BadRequest("Current password is incorrect.");
            }

            // Cập nhật mật khẩu mới
            user.Password = PasswordHashHandler.HashPassword(model.NewPassword);
            await _dbContext.SaveChangesAsync();

            return Ok("Password changed successfully.");
        }
    }
}

