namespace Archery.Models.DTO
{
    public class ArcherDTO
    {
        public int ArcherId { get; set; }
        public int UserId { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }

        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Status { get; set; }

        public int? AddressId { get; set; }
        public string? AddressLine { get; set; }

        public int? CountryId { get; set; }
        public string? Country { get; set; }

        public int? StateId { get; set; }
        public string? State { get; set; }

        public int? CityId { get; set; }
        public string? City { get; set; }

        public List<int>? EquipmentIds { get; set; }
        public List<string>? EquipmentNames { get; set; }

        public string? Role { get; set; }
    }
}
