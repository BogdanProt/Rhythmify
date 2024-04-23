using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Message
	{
		[Key]
		public int MessageID { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		[Required]
		public int ConversationID { get; set; }
		public virtual Conversation Conversation { get; set; }
		[Required]
		public int SenderID { get; set; }
		public virtual User Sender { get; set; }
		public int SongID { get; set; }
		public virtual Song Song { get; set; }
	}
}

