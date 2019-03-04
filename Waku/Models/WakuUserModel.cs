using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Waku.Data.Entities;

namespace Waku.Models
{
    public class WakuUserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}
