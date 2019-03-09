using System;
using System.ComponentModel.DataAnnotations;

namespace Waku.Data.Entities
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }

        public DateTime EditDate { get; set; }
    }
}
