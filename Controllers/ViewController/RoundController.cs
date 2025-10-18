using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    public class RoundController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}