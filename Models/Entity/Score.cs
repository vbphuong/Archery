using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("Score")]
    public class Score
    {
        [Key, Column(Order = 0)]
        public int RoundID { get; set; }

        [Key, Column(Order = 1)]
        public int CompetitionID { get; set; }

        [Key, Column(Order = 2)]
        public int ArcherID { get; set; }

        public int? TotalScore { get; set; }

        public TimeSpan? TimeRecorded { get; set; }

        public DateTime? DateRecorded { get; set; }

        [StringLength(10)]
        public string? ApprovalStatus { get; set; }

        //[ForeignKey("RoundID, CompetitionID")]
        public CompetitionRound CompetitionRound { get; set; } = null!;

        [ForeignKey("ArcherID")]
        public Archer Archer { get; set; } = null!;
    }
}
