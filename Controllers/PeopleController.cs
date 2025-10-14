using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    [Controller]
    public class PeopleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
