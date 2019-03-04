using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Waku.Data.Entities;
using System.Linq;

namespace Waku.Data
{
    public class WakuRepository : IWakuRepository
    {
        private readonly WakuContext context;
        private readonly ILogger logger;

        public WakuRepository(WakuContext context, ILogger<WakuRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public void AddEntity(object model)
        {
            context.Add(model);
        }

        public IEnumerable<BlogPost> GetAllBlogPosts()
        {
            try
            {
                var blogPosts = from p in context.BlogPosts
                                orderby p.EditDate descending
                                select p;
                return blogPosts.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get all blog posts: {ex}");
                return null;
            }
        }

        public BlogPost GetBlogPostById(int id)
        {
            var blogPosts = from p in context.BlogPosts
                            where p.Id == id
                            select p;
            return blogPosts.FirstOrDefault();
        }

        public IEnumerable<BlogPost> GetUserBlogPosts(string username)
        {
            var blogPosts = from p in context.BlogPosts
                            where p.User.UserName == username
                            select p;
            return blogPosts.ToList();
        }

        public bool SaveAll()
        {
            return context.SaveChanges() > 0;
        }
    }
}
