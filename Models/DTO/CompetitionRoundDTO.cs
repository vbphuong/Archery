namespace Archery.Models.DTO
{
    public class CompetitionRoundDTO
    {
        public int RoundID { get; set; }
        public int CompetitionID { get; set; }
        public string? RoundName { get; set; }           // hiển thị
        public string? EquivalentRoundName { get; set; } // hiển thị nếu có
        public string? Class { get; set; }
        public string? Gender { get; set; }
        public string? EquipmentName { get; set; }
        public string? HasEquivalent { get; set; }
        public string? IsPractice { get; set; }
        public string? Status { get; set; }
        public DateTime? TimeCreated { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}