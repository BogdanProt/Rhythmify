using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Feed
	{
		[Key]
		public int Id { get; set; }
		public DateTime Timestamp { get; set; }
		public virtual User User { get; set; }
		public ICollection<Post> Posts { get; set; }
	}
}

