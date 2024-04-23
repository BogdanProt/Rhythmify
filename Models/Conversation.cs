using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rhythmify.Models
{
	public class Conversation
	{
		[Key]
		public int ConversationID { get; set; }
		[Required]
		public int SenderID { get; set; }
		[NotMapped]
		public virtual User Sender { get; set; }
		[Required]
		public int ReceiverID { get; set; }
		[NotMapped]
		public virtual User Receiver { get; set; }
		public virtual ICollection<Message> Messages { get; set; }
	}
}

