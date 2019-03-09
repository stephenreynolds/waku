using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Waku.Data.Entities;
using Waku.Models;

namespace Waku.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IConfiguration config,
            IMapper mapper)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.config = config;
            this.mapper = mapper;

            EnsureCreated();
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(ClaimTypes.Name, user.UserName)
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            config["Tokens:Issuer"],
                            config["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: creds
                        );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created("", results);
                    }
                }
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityUser user = await userManager.FindByNameAsync(model.Username);

                    if (user == null)
                    {
                        user = mapper.Map<UserModel, IdentityUser>(model);

                        var result = await userManager.CreateAsync(user, model.Password);
                        if (result == IdentityResult.Success)
                        {
                            var userModel = mapper.Map<IdentityUser, UserModel>(user);

                            return Created($"/api/account/{userModel.Username}", userModel);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Failed to create user: {result.Errors.ToString()}");
                        }
                    }

                    return BadRequest("User already exists.");
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to create user: {ex}");
                return BadRequest($"Failed to create user");
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> RemoveUser(int id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());

                if (user != null)
                {
                    await userManager.DeleteAsync(user);
                    return Ok("User deleted successfully.");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to delete user: {ex}");
                return BadRequest("Failed to delete user");
            }
        }

        private void EnsureCreated()
        {
            string username = config["Defaults:User:Username"];
            string password = config["Defaults:User:Password"];

            logger.LogInformation($"Username: {username}, Password: {password}");

            UserModel userModel = new UserModel
            {
                Username = username,
                Password = password
            };

            CreateUser(userModel).Wait();
        }
    }
}
