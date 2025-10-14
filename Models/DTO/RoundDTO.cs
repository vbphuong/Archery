namespace Archery.Models.DTO
{
    public class RoundDTO
    {
        public int RoundID { get; set; }
        public string Name { get; set; } = null!;
        public string Class { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? EquipmentName { get; set; }
        public string? EquivalentRoundName { get; set; }
        public string? Active { get; set; }
        public DateTime? DateRecorded { get; set; }
        public TimeSpan? TimeRecorded { get; set; }
    }
}