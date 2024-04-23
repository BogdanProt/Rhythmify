using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Rhythmify.Models
{
	[PrimaryKey(nameof(FeedID), nameof(PostID))]
	public class FeedPosts
	{
		public int FeedID { get; set; }
		public virtual Feed Feed { get; set; }
		public int PostID { get; set; }
		public virtual Post Post { get; set; }
	}
}

