using Archery.Models.DTO;

namespace Archery.Models.Repository
{
    public interface ILocationHierarchyRepository
    {
        // Country
        Task<IEnumerable<CountryDTO>> GetCountriesAsync();
        Task<CountryDTO?> GetCountryAsync(int id);
        Task<CountryDTO> AddCountryAsync(CountryDTO dto);
        Task<CountryDTO?> UpdateCountryAsync(int id, CountryDTO dto);
        Task<bool> DeleteCountryAsync(int id);

        // State
        Task<IEnumerable<StateDTO>> GetStatesByCountryAsync(int countryId);
        Task<StateDTO> AddStateAsync(StateDTO dto);
        Task<bool> DeleteStateAsync(int id);

        // City
        Task<IEnumerable<CityDTO>> GetCitiesByStateAsync(int stateId);
        Task<CityDTO> AddCityAsync(CityDTO dto);
        Task<bool> DeleteCityAsync(int id);

        // Address
        Task<IEnumerable<AddressDTO>> GetAddressesByCityAsync(int cityId);
        Task<AddressDTO> AddAddressAsync(AddressDTO dto);
        Task<bool> DeleteAddressAsync(int id);
    }
}