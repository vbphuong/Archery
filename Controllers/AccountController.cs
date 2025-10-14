using Microsoft.AspNetCore.Mvc;
using Archery.Models.API;
using Archery.Models.Entity;

namespace Archery.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult Login()
		{
            ViewData["Layout"] = null;
            return View(new LoginRequestModel());
		}

		public IActionResult Register()
		{
            ViewData["Layout"] = null;
            return View(new User());
		}

        public IActionResult ResetPassword()
        {
            return View();
        }
    }
}

