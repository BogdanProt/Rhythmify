using System;
namespace Rhythmify.Models
{
	public class Connection
	{
		public Connection()
		{
		}

        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string FriendId { get; set; }
        public User Friend { get; set; }
    }
}

