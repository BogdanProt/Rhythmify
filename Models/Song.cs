using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Song
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public string Artist { get; set; }
		[Required]
		public string Genre { get; set; }
		public string PreviewUrl { get; set; }
		public string CoverPicture { get; set; }
		public double AverageRating { get; set; }
		public virtual ICollection<Rating> Ratings { get; set; }
		public virtual ICollection<Playlist> Playlists { get; set; }
		public virtual ICollection<Message> Messages { get; set; }
		public virtual ICollection<Post> Posts { get; set; }
	}
}
