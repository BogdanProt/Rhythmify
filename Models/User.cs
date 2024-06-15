using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;

namespace Rhythmify.Models
{
    public class User : IdentityUser
    {
        // variabila in care vom retine rolurile existente in baza de date
        // pentru popularea unui dropdown list
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
        public string? DisplayName { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Feed>? Feeds { get; set; }
        public virtual ICollection<Playlist>? Playlists { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
        public virtual ICollection<Conversation>? Conversations { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
    }
}
