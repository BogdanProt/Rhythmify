using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Rating
	{
		[Key]
		public int Id { get; set; }
		public int GivenRating { get; set; }
		[Required]
		public virtual User User { get; set; }
		[Required]
		public int SongId { get; set; }
		public virtual Song Song { get; set; }
	}
}

