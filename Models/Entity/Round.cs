using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("Round")]
    public class Round
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoundID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Class { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = null!; 

        public int? EquipmentID { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment? Equipment { get; set; } = null!;
    }
}