namespace Archery.Models.DTO
{
    public class CompetitionDTO
    {
        public int CompetitionID { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? IsChampionship { get; set; }
    }
}