using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("CompetitionRound")]
    public class CompetitionRound
    {
        [Key, Column(Order = 0)]
        public int RoundID { get; set; }

        [Key, Column(Order = 1)]
        public int CompetitionID { get; set; }

        [StringLength(10)]
        public string? HasEquivalent { get; set; }

        [StringLength(10)]
        public string? IsPractice { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public DateTime? TimeCreated { get; set; }
        public DateTime? DateCreated { get; set; }

        [ForeignKey("RoundID")]
        public Round Round { get; set; } = null!;

        [ForeignKey("CompetitionID")]
        public Competition Competition { get; set; } = null!;
    }
}
