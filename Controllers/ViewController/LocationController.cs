using Archery.Data;
using Archery.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Archery.Controllers
{
    [Authorize] 
    public class LocationController : Controller
    {
        private readonly AppDbContext _context;

        public LocationController(AppDbContext context)
        {
            _context = context;
        }

        // COUNTRY
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var countries = await _context.Countries
                .OrderBy(c => c.CountryName)
                .ToListAsync();
            return View(countries);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateCountry() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCountry(Country model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.Countries.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //STATE
        [AllowAnonymous]
        public async Task<IActionResult> States(int countryId)
        {
            var country = await _context.Countries
                .Include(c => c.States)
                .FirstOrDefaultAsync(c => c.CountryID == countryId);

            if (country == null) return NotFound();

            ViewBag.CountryId = country.CountryID;
            ViewBag.CountryName = country.CountryName;
            return View(country.States);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateState(int countryId)
        {
            ViewBag.CountryId = countryId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateState(State model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.States.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(States), new { countryId = model.CountryID });
        }

        // CITY 
        [AllowAnonymous]
        public async Task<IActionResult> Cities(int stateId)
        {
            var state = await _context.States
                .Include(s => s.Cities)
                .FirstOrDefaultAsync(s => s.StateID == stateId);

            if (state == null) return NotFound();

            ViewBag.StateId = state.StateID;
            ViewBag.StateName = state.StateName;
            ViewBag.CountryId = state.CountryID;
            return View(state.Cities);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateCity(int stateId)
        {
            ViewBag.StateId = stateId;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCity(City model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Cities.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cities), new { stateId = model.StateID });
        }

        // ADDRESS (optional hiển thị) 
        //[AllowAnonymous]
        //public async Task<IActionResult> Addresses(int cityId)
        //{
        //    var city = await _context.Cities
        //        .Include(c => c.Addresses)
        //        .FirstOrDefaultAsync(c => c.CityID == cityId);

        //    if (city == null) return NotFound();

        //    ViewBag.CityName = city.CityName;
        //    return View(city.Addresses);
        //}
    }
}
