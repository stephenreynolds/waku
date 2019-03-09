using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Waku.Data.Entities;

namespace Waku.Data
{
    public class WakuContext : IdentityDbContext<IdentityUser>
    {
        public WakuContext(DbContextOptions<WakuContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}
