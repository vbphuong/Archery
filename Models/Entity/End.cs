using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("End")]
    public class End
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EndID { get; set; }

        [Required]
        public int RangeID { get; set; }

        [StringLength(5)]
        public string? EndNumber { get; set; }

        [ForeignKey("RangeID")]
        public ArcheryRange? ArcheryRange { get; set; } = null!;

        public ICollection<Arrow> Arrows { get; set; } = new List<Arrow>();
    }
}