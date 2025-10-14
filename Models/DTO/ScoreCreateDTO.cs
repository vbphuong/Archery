namespace Archery.Models.DTO
{
    public class ScoreCreateDTO
    {
        public int RoundID { get; set; }
        public int CompetitionID { get; set; }
        public int ArcherID { get; set; }
    }
}