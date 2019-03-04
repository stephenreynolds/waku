using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Waku.Data;
using Waku.Data.Entities;
using Waku.Models;

namespace Waku.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Produces("application/json")]
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
        public IActionResult Get()
        {
            try
            {
                var username = User.Identity.Name;
                var results = repository.GetUserBlogPosts(username);
                return Ok(mapper.Map<IEnumerable<BlogPost>, IEnumerable<BlogPostModel>>(results));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get user blog posts: {ex}");
                return BadRequest("Failed to get user blog posts.");
            }
        }

        [HttpGet("id:int")]
        public IActionResult Get(int id)
        {
            try
            {
                var username = User.Identity.Name;
                var result = repository.GetBlogPostById(username, id);
                return Ok (mapper.Map<BlogPost, BlogPostModel>(result));
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to get blog post: {ex}");
                return BadRequest("Failed to get blog post.");
            }
        }
    }
}
