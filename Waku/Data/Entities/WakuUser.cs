using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Waku.Data.Entities
{
    [DataContract]
    public class WakuUser : IdentityUser
    {
        [DataMember]
        public override string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<BlogPost> BlogPosts { get; set; }
    }
}
