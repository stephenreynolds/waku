using System.ComponentModel.DataAnnotations;

namespace Waku.Models
{
    public class PaginationModel
    {
        [Required]
        public int Page { get; set; }

        [Required]
        public int Size { get; set; }
    }
}
