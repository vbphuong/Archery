using Archery.Models.DTO;
using Archery.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archery.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LocationHierarchyAPIController : ControllerBase
    {
        private readonly ILocationHierarchyRepository _repo;

        public LocationHierarchyAPIController(ILocationHierarchyRepository repo)
        {
            _repo = repo;
        }

        //COUNTRY
        [HttpGet]
        public async Task<IActionResult> GetCountries() => Ok(await _repo.GetCountriesAsync());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCountry([FromBody] CountryDTO dto)
            => Ok(await _repo.AddCountryAsync(dto));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] CountryDTO dto)
        {
            var result = await _repo.UpdateCountryAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var ok = await _repo.DeleteCountryAsync(id);
            if (!ok) return NotFound();
            return Ok();
        }

        // STATE
        [HttpGet("{countryId}")]
        public async Task<IActionResult> GetStatesByCountry(int countryId)
            => Ok(await _repo.GetStatesByCountryAsync(countryId));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddState([FromBody] StateDTO dto)
            => Ok(await _repo.AddStateAsync(dto));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteState(int id)
        {
            var ok = await _repo.DeleteStateAsync(id);
            if (!ok) return NotFound();
            return Ok();
        }

        //CITY
        [HttpGet("{stateId}")]
        public async Task<IActionResult> GetCitiesByState(int stateId)
            => Ok(await _repo.GetCitiesByStateAsync(stateId));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCity([FromBody] CityDTO dto)
            => Ok(await _repo.AddCityAsync(dto));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var ok = await _repo.DeleteCityAsync(id);
            if (!ok) return NotFound();
            return Ok();
        }

        //ADDRESS
        [HttpGet("by-city/{cityId}")]
        public async Task<IActionResult> GetAddressesByCity(int cityId)
            => Ok(await _repo.GetAddressesByCityAsync(cityId));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDTO dto)
            => Ok(await _repo.AddAddressAsync(dto));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var ok = await _repo.DeleteAddressAsync(id);
            if (!ok) return NotFound();
            return Ok();
        }
    }
}