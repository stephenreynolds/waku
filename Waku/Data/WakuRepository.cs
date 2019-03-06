using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Waku.Data.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

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

        public EntityEntry AddEntity(object model)
        {
            return context.Add(model);
        }

        public EntityEntry RemoveEntity(object model)
        {
            return context.Remove(model);
        }

        public EntityEntry UpdateBlogPost(BlogPost model)
        {
            var local = context.Set<BlogPost>().Local.FirstOrDefault(entry => entry.Id.Equals(model.Id));
            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
            else
            {
                throw new InvalidOperationException("Blog post does not exist in the database.");
            }

            return context.Update(model);
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
            return context.BlogPosts.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<BlogPost> GetUserBlogPosts(string username)
        {
            return context.BlogPosts.Where(p => p.User.UserName == username);
        }

        public bool SaveAll()
        {
            return context.SaveChanges() > 0;
        }
    }
}
