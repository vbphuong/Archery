using System;
namespace Archery.Models.API
{
	public class LoginResponseModel
	{
		public string? Email { get; set; }
		public string? AccessToken { get; set; }
		public int ExpiredIn { get; set; }
	}
}

