using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Waku.Data.Entities;

namespace Waku.Data
{
    public interface IWakuRepository
    {
        bool SaveAll();
        EntityEntry AddEntity(object model);
        EntityEntry RemoveEntity(object model);

        EntityEntry UpdateBlogPost(BlogPost model);
        IEnumerable<BlogPost> GetAllBlogPosts();
        IEnumerable<BlogPost> GetBlogPostsInRange(int start, int end);
        BlogPost GetBlogPostById(int id);
        int GetBlogPostCount();
    }
}
