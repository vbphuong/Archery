using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("Arrow")]
    public class Arrow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArrowID { get; set; }

        [Required]
        public int EndID { get; set; }

        [StringLength(3)]
        public string? Value { get; set; }

        [ForeignKey("EndID")]
        public End? End { get; set; } = null!;
    }
}