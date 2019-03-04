using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly SignInManager<WakuUser> signInManager;
        private readonly UserManager<WakuUser> userManager;
        private readonly IConfiguration config;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<WakuUser> signInManager,
            UserManager<WakuUser> userManager,
            IConfiguration config)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.config = config;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Username, model.Password, model.RememberMe, lockoutOnFailure : false);

                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"]);
                    }
                    else
                    {
                        return RedirectToAction("Index", "App");
                    }
                }
            }

            ModelState.AddModelError("", "Failed to login");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "App");
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure : false);

                    if (result.Succeeded)
                    {
                        var claims = new []
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                            issuer: config["Token:Issuer"],
                            audience : config["Token:Audience"],
                            claims : claims,
                            expires : DateTime.UtcNow.AddMinutes(30),
                            signingCredentials : creds
                        );

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expirection = token.ValidTo
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
            WakuUser user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                user = new WakuUser
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result == IdentityResult.Success)
                {
                    if (model.Role == "Admin")
                    {
                        AddUserToRoleAuthed(user, model.Role);
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(user, model.Role);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Failed to create new user: {result.Errors.ToString()}");
                }
            }

            return BadRequest("Failed to create user.");
        }

        [Authorize(Roles = "Admin")]
        private async void AddUserToRoleAuthed(WakuUser user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
