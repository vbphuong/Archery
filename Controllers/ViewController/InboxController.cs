using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Archery.Data;

namespace Archery.Controllers
{
    public class InboxController : Controller
    {
        private readonly AppDbContext _context;

        public InboxController(AppDbContext context)
        {
            _context = context;
        }
 
        public IActionResult Index()
        {
            var users = _context.User
                .Select(u => new SelectListItem
                {
                    Value = u.UserID.ToString(),
                    Text = (u.FirstName ?? "") + " " + (u.LastName ?? "")
                })
                .ToList();

            ViewBag.Users = users;
            return View();
        }

    }
}
