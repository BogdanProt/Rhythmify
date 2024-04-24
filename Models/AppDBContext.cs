using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace Rhythmify.Models
{
	public class AppDBContext : DbContext
	{
		public AppDBContext()
		{
		}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=RhythmifyDB;Integrated Security=True;Multiple Active Result Sets=True";

            //WINDOWS:
            optionsBuilder.UseSqlServer(connectionString);

            //MAC:
            //var serverVersion = new MySqlServerVersion(new Version(8, 3, 0));

            //optionsBuilder.UseMySql(connectionString, serverVersion);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<PlaylistSongs> PlaylistSongs { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<FeedPosts> FeedPosts { get; set; }
    }
}