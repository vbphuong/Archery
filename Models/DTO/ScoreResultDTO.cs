namespace Archery.Models.DTO
{
    public class ScoreResultDTO
    {
        public int ArcherID { get; set; }
        public int RoundID { get; set; }
        public int CompetitionID { get; set; }
        public int? TotalScore { get; set; }
        public DateTime? DateRecorded { get; set; }
        public TimeSpan? TimeRecorded { get; set; }
    }
}

