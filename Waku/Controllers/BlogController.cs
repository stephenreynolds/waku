using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Waku.Data;
using Waku.Data.Entities;
using Waku.Models;

namespace Waku.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Moderator,Author")]
    public class BlogController : Controller
    {
        private readonly IWakuRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<BlogController> logger;
        private readonly UserManager<WakuUser> userManager;

        public BlogController(
            IWakuRepository repository,
            IMapper mapper,
            ILogger<BlogController> logger,
            UserManager<WakuUser> userManager)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
            this.userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                var results = repository.GetAllBlogPosts();
                return Ok(mapper.Map<IEnumerable<BlogPost>, IEnumerable<BlogPostModel>>(results));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get user blog posts: {ex}");
                return BadRequest("Failed to get user blog posts.");
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public IActionResult Get(int id)
        {
            try
            {
                var username = User.Identity.Name;
                var result = repository.GetBlogPostById(id);
                return Ok(mapper.Map<BlogPost, BlogPostModel>(result));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get blog post: {ex}");
                return BadRequest("Failed to get blog post.");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePost([FromBody] BlogPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newPost = mapper.Map<BlogPostModel, BlogPost>(model);
                    newPost.User = await userManager.FindByNameAsync(User.Identity.Name);
                    newPost.PublishDate = DateTime.UtcNow;
                    newPost.EditDate = DateTime.MinValue;

                    repository.AddEntity(newPost);
                    if (repository.SaveAll())
                    {
                        var postModel = mapper.Map<BlogPost, BlogPostModel>(newPost);
                        return Created($"/api/blog/{postModel.Id}", postModel);
                    }
                    else
                    {
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to create post: {ex}");
                return BadRequest("Failed to create post");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> EditPost([FromBody] BlogPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var post = repository.GetBlogPostById(model.Id);

                    if (post != null)
                    {
                        // Make sure user is the correct author (unless admin or mod).
                        var user = await userManager.FindByNameAsync(User.Identity.Name);
                        if (await userManager.IsInRoleAsync(user, "Author") && user.Id != post.User.Id)
                        {
                            return Unauthorized("You do not have permission to edit this post.");
                        }

                        DateTime publishDate = post.PublishDate; // Make sure publish date is not changed.
                        post = mapper.Map<BlogPostModel, BlogPost>(model);
                        post.PublishDate = publishDate;
                        post.EditDate = DateTime.UtcNow;

                        repository.UpdateBlogPost(post);
                        if (repository.SaveAll())
                        {
                            var postModel = mapper.Map<BlogPost, BlogPostModel>(post);
                            return Ok(postModel);
                        }
                        else
                        {
                            return BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("The post does not exist.");
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to edit post: {ex}");
                return BadRequest("Failed to edit post");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var post = repository.GetBlogPostById(id);

                if (post != null)
                {
                    // Make sure user is the correct author (unless admin or mod).
                    var user = await userManager.FindByNameAsync(User.Identity.Name);
                    if (await userManager.IsInRoleAsync(user, "Author") && user.Id != post.User.Id)
                    {
                        return Unauthorized("You do not have permission to delete this post.");
                    }

                    repository.RemoveEntity(post);
                    if (repository.SaveAll())
                    {
                        return Ok("Post deleted successfully.");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete post: {ex}");
                return BadRequest("Failed to delete post");
            }
        }
    }
}
