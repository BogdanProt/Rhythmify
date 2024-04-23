using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Feed
	{
		[Key]
		public int FeedID { get; set; }
		public DateTime Timestamp { get; set; }
		public int UserID { get; set; }
		public virtual User User { get; set; }
		public ICollection<FeedPosts> FeedPosts { get; set; }
	}
}

