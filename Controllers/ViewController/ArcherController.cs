using Archery.Models.DTO;
using Archery.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Archery.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    [Controller]
    public class ArcherController : Controller
    {
        private readonly IArcherRepository _repo;

        //public ArcherController(IArcherRepository repo)
        //{
        //    _repo = repo;
        //}

        [HttpGet("Details/{userId}")]
        public async Task<IActionResult> Details(int userId)
        {
            Console.WriteLine($"Hit ArcherController.Details(userId={userId})");

            var archer = await _repo.GetByUserIdAsync(userId);

            // Nếu chưa có, tạo mới 1 object trống (để View hiển thị form)
            if (archer == null)
            {
                archer = new ArcherDTO
                {
                    UserId = userId
                };
                Console.WriteLine("Archer not found — returning empty model for creation");
            }

            return View(archer);
        }

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public ArcherController(IArcherRepository repo, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _repo = repo;
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration.GetValue<string>("ApiBaseUrl")?.TrimEnd('/') ?? "";
        }

        [HttpGet("EliteList")]
        public async Task<IActionResult> EliteList()
        {
            var client = _httpClientFactory.CreateClient();
            // nếu bạn dùng Bearer token, gán header ở đây (bằng session/localstorage từ frontend)
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync($"{_apiBaseUrl}/api/ArcherAPI/EliteArchers");
            if (!res.IsSuccessStatusCode)
            {
                // fallback: show empty view or error
                return View(new List<EliteArcherDTO>());
            }

            var json = await res.Content.ReadAsStringAsync();
            var elites = JsonConvert.DeserializeObject<List<EliteArcherDTO>>(json) ?? new List<EliteArcherDTO>();

            return View(elites);
        }
    }
}
