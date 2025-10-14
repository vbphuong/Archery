using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("Equipment")]
    public class Equipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EquipmentID { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = null!;

        public ICollection<ArcherEquipment> ArcherEquipments { get; set; } = new List<ArcherEquipment>();
    }
}