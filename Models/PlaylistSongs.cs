using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rhythmify.Models
{
    [PrimaryKey(nameof(PlaylistID), nameof(SongID))]
    public class PlaylistSongs
	{
		public int PlaylistID { get; set; }
		public virtual Playlist Playlist { get; set; }
		public int SongID { get; set; }
		public virtual Song Song { get; set; }
    }
}

