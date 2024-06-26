﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Rhythmify.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}
