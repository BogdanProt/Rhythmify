using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Post
	{
		[Key]
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		[Required]
		public virtual User User { get; set; }
		[Required]
		public int SongId { get; set; }
		public virtual Song Song { get; set; }
		public virtual ICollection<Feed> Feeds { get; set; }
	}
}

