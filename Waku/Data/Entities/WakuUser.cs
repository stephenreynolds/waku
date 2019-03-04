using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Waku.Data.Entities
{
    public class WakuUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<BlogPost> BlogPosts { get; set; }
    }
}
