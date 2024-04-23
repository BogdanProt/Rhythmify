using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Playlist
	{
		[Key]
		public int PlaylistID { get; set; }
		[Required]
		public string Title { get; set; }
		public string Description { get; set; }
		[Required(ErrorMessage = "User-ul este obligatoriu!")]
		public int UserID { get; set; }
		public virtual User User { get; set; }
		public virtual ICollection<PlaylistSongs> PlaylistSongs { get; set; }
		
	}
}
