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

        public string SubTitle { get; set; }

        public DateTime PublishDate { get; set; }

        public DateTime EditDate { get; set; }

        public string Thumbnail { get; set; }
    }
}
