using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class Playlist
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public string Description { get; set; }
		[Required(ErrorMessage = "User-ul este obligatoriu!")]
		public virtual User User { get; set; }
		public virtual ICollection<Song> Songs { get; set; }
		
	}
}
