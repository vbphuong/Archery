namespace Archery.Models.DTO
{
    public class CountryDTO
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }

    public class StateDTO
    {
        public int StateID { get; set; }
        public string StateName { get; set; } = string.Empty;
        public int CountryID { get; set; }
        public string? CountryName { get; set; }
    }

    public class CityDTO
    {
        public int CityID { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string? ZipCode { get; set; }
        public int StateID { get; set; }
        public string? StateName { get; set; }
    }

    public class AddressDTO
    {
        public int AddressID { get; set; }
        public string? AddressLine { get; set; }
        public string? Ward { get; set; }
        public string? District { get; set; }
        public int CityID { get; set; }
        public string? CityName { get; set; }
    }
}