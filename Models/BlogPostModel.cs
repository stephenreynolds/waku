using System;
using System.ComponentModel.DataAnnotations;
using Waku.Data.Entities;

namespace Waku.Models
{
    public class BlogPostModel
    {
        public int Id { get; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime EditDate { get; set; }

        public string Image { get; set; }
    }
}
