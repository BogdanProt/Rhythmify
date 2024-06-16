namespace Rhythmify.Models
{
    public class PlaylistSong
    {
        public int PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        public int SongId { get; set; }
        public virtual Song Song { get; set; }
        public DateTime TimeAdded { get;set; }
    }
}
