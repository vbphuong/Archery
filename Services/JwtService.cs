using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Archery.Data;
using Archery.Handlers;
using Archery.Models.API;

namespace Archery.Services
{
	public class JwtService
	{
		private readonly IConfiguration _configuration;
		private readonly AppDbContext _dbContext;

		public JwtService(IConfiguration configuration, AppDbContext dbContext)
		{
			_configuration = configuration;
			_dbContext = dbContext;
            Console.WriteLine(">>> JwtService instance created: " + Guid.NewGuid());
        }

		public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
		{
			if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
				return null;

			var user = await _dbContext.User
				.Include(u => u.Role)
				.FirstOrDefaultAsync(x => x.Email == request.Email);

			if (user == null || !PasswordHashHandler.VerifyPassword(request.Password, user.Password))
				return null;

			var issuer = _configuration["JwtConfig:Issuer"];
			var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"]
			  ?? throw new InvalidOperationException("JWT Key not configured.");

            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, request.Email),
					new Claim(ClaimTypes.Role, user.Role!.RoleName)
				}),
				Expires = DateTime.UtcNow.AddMinutes(tokenValidityMins),
				Issuer = issuer,
				Audience = audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
					SecurityAlgorithms.HmacSha512Signature)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new LoginResponseModel
			{
				Email = request.Email,
				AccessToken = tokenHandler.WriteToken(token),
				ExpiredIn = (int)DateTime.UtcNow.AddMinutes(tokenValidityMins).Subtract(DateTime.UtcNow).TotalSeconds
			};
        }
	}
}

