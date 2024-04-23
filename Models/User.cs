using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
	public class User
	{
		[Key]
		public int UserID { get; set; }
		[Required(ErrorMessage = "Email-ul este obligatoriu!")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Parola este obligatorie!")]
		public string Password { get; set; }
		public string ProfilePicture { get; set; }
		public string Description { get; set; }
		public virtual ICollection<Playlist> Playlists { get; set; }
		public virtual ICollection<Conversation> Conversations { get; set; }
		public virtual ICollection<Post> Posts { get; set; }
	}
}

