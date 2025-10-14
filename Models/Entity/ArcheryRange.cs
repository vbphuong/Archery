using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("ArcheryRange")]
    public class ArcheryRange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RangeID { get; set; }

        [Required]
        public int RoundID { get; set; }

        [Required]
        public int CompetitionID { get; set; }

        [Required]
        public int ArcherID { get; set; }

        [Required]
        [StringLength(10)]
        public string Distance { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string TargetFace { get; set; } = null!;

        [StringLength(5)]
        public string? NumEnds { get; set; }

        //[ForeignKey("RoundID, CompetitionID, ArcherID")]
        public Score Score { get; set; } = null!;

        public ICollection<End> Ends { get; set; } = new List<End>();
    }
}
