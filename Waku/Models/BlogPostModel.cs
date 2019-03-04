using System;
using System.ComponentModel.DataAnnotations;
using Waku.Data.Entities;

namespace Waku.Models
{
    public class BlogPostModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EditDate { get; set; } = DateTime.UtcNow;

        [Required]
        public WakuUser User { get; set; }
    }
}
