using System;
using System.ComponentModel.DataAnnotations;
using Waku.Data.Entities;

namespace Waku.Models
{
    public class BlogPostModel
    {
        public int Id { get; }
        public WakuUser User { get; }
        public DateTime PublishDate { get; }
        public DateTime EditDate { get; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
