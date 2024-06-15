using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rhythmify.Models
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? UserName { get; set; }
        public string? User1Id { get; set; }
        [NotMapped]
        public virtual User? User1 { get; set; }
        [Required]
        public string? User2Id { get; set; }
        [NotMapped]
        public virtual User? User2 { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
