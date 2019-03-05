using System.Collections.Generic;
using Waku.Data.Entities;

namespace Waku.Data
{
    public interface IWakuRepository
    {
        bool SaveAll();
        void AddEntity(object model);
        void UpdateEntity(object model);
        void RemoveEntity(object model);

        IEnumerable<BlogPost> GetAllBlogPosts();
        IEnumerable<BlogPost> GetUserBlogPosts(string username);
        BlogPost GetBlogPostById(int id);
    }
}
