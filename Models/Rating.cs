using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Rating
	{
		[Key]
		public int RatingID { get; set; }
		public int GivenRating { get; set; }
		[Required]
		public int UserID { get; set; }
		public virtual User User { get; set; }
		[Required]
		public int SongID { get; set; }
		public virtual Song Song { get; set; }
	}
}

