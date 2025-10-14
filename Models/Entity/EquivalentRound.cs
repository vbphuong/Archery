using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("EquivalentRound")]
    public class EquivalentRound
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquivalentID { get; set; }

        public int? BaseRoundID { get; set; }

        public int? EquivalentRoundID { get; set; }

        public TimeSpan? TimeRecorded { get; set; }

        public DateTime? DateRecorded { get; set; }

        [Required]
        [StringLength(10)]
        public string Active { get; set; } = "Yes";

        [ForeignKey("BaseRoundID")]
        public Round? BaseRound { get; set; } = null!;

        [ForeignKey("EquivalentRoundID")]
        public Round? EquivalentTo { get; set; } = null!;
    }
}
