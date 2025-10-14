using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("Country")]
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryID { get; set; }

        [Required, StringLength(100)]
        public string CountryName { get; set; } = null!;

        public ICollection<State> States { get; set; } = new List<State>();
    }

    [Table("State")]
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateID { get; set; }

        [Required, StringLength(100)]
        public string StateName { get; set; } = null!;

        [ForeignKey("Country")]
        public int CountryID { get; set; }
        public Country? Country { get; set; }

        public ICollection<City> Cities { get; set; } = new List<City>();
    }

    [Table("City")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityID { get; set; }

        [Required, StringLength(100)]
        public string CityName { get; set; } = null!;

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [ForeignKey("State")]
        public int StateID { get; set; }
        public State? State { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }

    [Table("Address")]
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AddressID { get; set; }

        [StringLength(255)]
        public string? AddressLine { get; set; }

        [StringLength(100)]
        public string? Ward { get; set; }

        [StringLength(100)]
        public string? District { get; set; }

        [ForeignKey("City")]
        public int CityID { get; set; }
        public City? City { get; set; }
    }
}
