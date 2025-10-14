using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("Archer")]
    public class Archer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArcherID { get; set; }

        public int UserID { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string? Gender { get; set; }

        public int? AddressID { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string? Status { get; set; }

        [ForeignKey("AddressID")]
        public Address? Address { get; set; }

        [ForeignKey("UserID")]
        public User? User { get; set; }

        public ICollection<ArcherEquipment> ArcherEquipments { get; set; } = new List<ArcherEquipment>();
    }
}