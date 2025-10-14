using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("ArcherEquipment")]
    public class ArcherEquipment
    {
        public int EquipmentID { get; set; }
        public int ArcherID { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment Equipment { get; set; } = null!;

        [ForeignKey("ArcherID")]
        public Archer Archer { get; set; } = null!;
    }
}