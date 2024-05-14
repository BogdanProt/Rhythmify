using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Models;
using System.Threading.Channels;

namespace Rhythmify.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Feed> Feeds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=RhythmifyDB;Integrated Security=True;Multiple Active Result Sets=True";

            //var serverVersion = new MySqlServerVersion(new Version(8, 3, 0));

            //WINDOWS:
            optionsBuilder.UseSqlServer(connectionString);

            //optionsBuilder.UseMySql(connectionString, serverVersion);
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // definire primary key compus
            /*
             modelBuilder.Entity<User>()
            .HasKey(ab => new {
                ab.Id,
                ab.UserId,
                ab.ChannelId
            });*/
         
        }
    }
}