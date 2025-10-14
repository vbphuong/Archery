using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Archery.Models.Entity
{
    [Table("Inbox")]
    public class Inbox
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InboxID { get; set; }

        [Required]
        public int Receiver { get; set; }

        [ForeignKey("Receiver")]
        public User ReceiverUser { get; set; } = null!;

        [Required]
        public string Message { get; set; } = null!;

        [Required]
        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; } = false;

        [StringLength(50)]
        public string Status { get; set; } = null!;

        [Required]
        public int Sender { get; set; }

        [ForeignKey("Sender")]
        public User SenderUser { get; set; } = null!;
    }
}