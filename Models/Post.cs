using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Post
	{
		[Key]
		public int PostID { get; set; }
		public string Content { get; set; }
		public DateTime Timestamp { get; set; }
		[Required]
		public int UserID { get; set; }
		public virtual User User { get; set; }
		[Required]
		public int SongID { get; set; }
		public virtual Song Song { get; set; }
		public virtual ICollection<FeedPosts> FeedPosts { get; set; }
	}
}

