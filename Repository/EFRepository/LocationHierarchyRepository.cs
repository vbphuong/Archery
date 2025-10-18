using Archery.Models.Entity;
using Archery.Models.DTO;
using Archery.Data;
using Microsoft.EntityFrameworkCore;

namespace Archery.Repository
{
    public class LocationHierarchyRepository : BaseRepository, ILocationHierarchyRepository
    {
        public LocationHierarchyRepository(AppDbContext context) : base(context) { }

        // COUNTRY
        public async Task<IEnumerable<CountryDTO>> GetCountriesAsync()
        {
            return await _context.Countries
                .Select(c => new CountryDTO { CountryID = c.CountryID, CountryName = c.CountryName })
                .ToListAsync();
        }

        public async Task<CountryDTO?> GetCountryAsync(int id)
        {
            return await _context.Countries
                .Where(c => c.CountryID == id)
                .Select(c => new CountryDTO { CountryID = c.CountryID, CountryName = c.CountryName })
                .FirstOrDefaultAsync();
        }

        public async Task<CountryDTO> AddCountryAsync(CountryDTO dto)
        {
            var entity = new Country { CountryName = dto.CountryName };
            _context.Countries.Add(entity);
            await _context.SaveChangesAsync();
            dto.CountryID = entity.CountryID;
            return dto;
        }

        public async Task<CountryDTO?> UpdateCountryAsync(int id, CountryDTO dto)
        {
            var entity = await _context.Countries.FindAsync(id);
            if (entity == null) return null;
            entity.CountryName = dto.CountryName;
            await _context.SaveChangesAsync();
            dto.CountryID = entity.CountryID;
            return dto;
        }

        public async Task<bool> DeleteCountryAsync(int id)
        {
            var entity = await _context.Countries.FindAsync(id);
            if (entity == null) return false;
            _context.Countries.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // STATE
        public async Task<IEnumerable<StateDTO>> GetStatesByCountryAsync(int countryId)
        {
            return await _context.States
                .Where(s => s.CountryID == countryId)
                .Select(s => new StateDTO
                {
                    StateID = s.StateID,
                    StateName = s.StateName,
                    CountryID = s.CountryID,
                    CountryName = s.Country!.CountryName
                })
                .ToListAsync();
        }

        public async Task<StateDTO> AddStateAsync(StateDTO dto)
        {
            var entity = new State { StateName = dto.StateName, CountryID = dto.CountryID };
            _context.States.Add(entity);
            await _context.SaveChangesAsync();
            dto.StateID = entity.StateID;
            return dto;
        }

        public async Task<bool> DeleteStateAsync(int id)
        {
            var entity = await _context.States.FindAsync(id);
            if (entity == null) return false;
            _context.States.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        //CITY 
        public async Task<IEnumerable<CityDTO>> GetCitiesByStateAsync(int stateId)
        {
            return await _context.Cities
                .Where(c => c.StateID == stateId)
                .Select(c => new CityDTO
                {
                    CityID = c.CityID,
                    CityName = c.CityName,
                    ZipCode = c.ZipCode,
                    StateID = c.StateID,
                    StateName = c.State!.StateName
                })
                .ToListAsync();
        }

        public async Task<CityDTO> AddCityAsync(CityDTO dto)
        {
            var entity = new City
            {
                CityName = dto.CityName,
                ZipCode = dto.ZipCode,
                StateID = dto.StateID
            };
            _context.Cities.Add(entity);
            await _context.SaveChangesAsync();
            dto.CityID = entity.CityID;
            return dto;
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            var entity = await _context.Cities.FindAsync(id);
            if (entity == null) return false;
            _context.Cities.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<State?> GetStateByIdAsync(int id)
            => await _context.States.FindAsync(id);

        // ADDRESS
        public async Task<IEnumerable<AddressDTO>> GetAddressesByCityAsync(int cityId)
        {
            return await _context.Addresses
                .Where(a => a.CityID == cityId)
                .Select(a => new AddressDTO
                {
                    AddressID = a.AddressID,
                    AddressLine = a.AddressLine,
                    Ward = a.Ward,
                    District = a.District,
                    CityID = a.CityID,
                    CityName = a.City!.CityName
                })
                .ToListAsync();
        }

        public async Task<AddressDTO> AddAddressAsync(AddressDTO dto)
        {
            var entity = new Address
            {
                AddressLine = dto.AddressLine,
                Ward = dto.Ward,
                District = dto.District,
                CityID = dto.CityID
            };
            _context.Addresses.Add(entity);
            await _context.SaveChangesAsync();
            dto.AddressID = entity.AddressID;
            return dto;
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var entity = await _context.Addresses.FindAsync(id);
            if (entity == null) return false;
            _context.Addresses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}