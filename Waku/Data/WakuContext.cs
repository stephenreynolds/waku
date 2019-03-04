using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Waku.Data.Entities;

namespace Waku.Data
{
    public class WakuContext : IdentityDbContext<WakuUser>
    {
        public WakuContext(DbContextOptions<WakuContext> options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}
