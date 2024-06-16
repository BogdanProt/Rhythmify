using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		public virtual ICollection<Rating>? Ratings { get; set; }
		public virtual ICollection<PlaylistSong>? PlaylistSongs { get; set; }
		public virtual ICollection<Message>? Messages { get; set; }
		public virtual ICollection<Post>? Posts { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllUserPlaylists { get; set; }
    }
}
