using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Waku.Data.Entities
{
    public class WakuUser : IdentityUser
    {
        [Required]
        public string Role { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<BlogPost> BlogPosts { get; set; }
    }
}
