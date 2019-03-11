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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BlogController : Controller
    {
        private readonly IWakuRepository repository;
        private readonly IMapper mapper;
        private readonly ILogger<BlogController> logger;

        public BlogController(
            IWakuRepository repository,
            IMapper mapper,
            ILogger<BlogController> logger)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.logger = logger;
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

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult Get(int id)
        {
            try
            {
                var result = repository.GetBlogPostById(id);
                return Ok(mapper.Map<BlogPost, BlogPostModel>(result));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get blog post: {ex}");
                return BadRequest("Failed to get blog post.");
            }
        }

        [HttpGet]
        public IActionResult Count()
        {
            return Ok(repository.GetBlogPostCount());
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult GetPage([FromBody] PaginationModel model)
        {
            int start = (model.Page - 1) * model.Size;
            int end = start + model.Size;

            try
            {
                var result = repository.GetBlogPostsInRange(start, end);
                return Ok(mapper.Map<IEnumerable<BlogPost>, IEnumerable<BlogPostModel>>(result));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get blog posts in range ({start}-{end}): {ex}");
                return BadRequest($"Faield to get blog posts in range ({start}-{end}");
            }
        }

        [HttpPost("[action]")]
        public IActionResult CreatePost([FromBody] BlogPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newPost = mapper.Map<BlogPostModel, BlogPost>(model);
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
        public IActionResult EditPost([FromBody] BlogPostModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var post = repository.GetBlogPostById(model.Id);

                    if (post != null)
                    {
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
        public IActionResult Delete(int id)
        {
            try
            {
                var post = repository.GetBlogPostById(id);

                if (post != null)
                {
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
