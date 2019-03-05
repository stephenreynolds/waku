using System;
using System.IdentityModel.Tokens.Jwt;
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
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly SignInManager<WakuUser> signInManager;
        private readonly UserManager<WakuUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration config;
        private readonly IMapper mapper;

        public AccountController(
            ILogger<AccountController> logger,
            SignInManager<WakuUser> signInManager,
            UserManager<WakuUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            IMapper mapper)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.config = config;
            this.mapper = mapper;

            EnsureRolesCreated().Wait();
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
            try
            {
                if (ModelState.IsValid)
                {
                    WakuUser user = await userManager.FindByNameAsync(model.Username);

                    if (user == null)
                    {
                        // TODO: salt passwords.
                        user = mapper.Map<UserModel, WakuUser>(model);

                        var result = await userManager.CreateAsync(user, model.Password);
                        if (result == IdentityResult.Success)
                        {
                            switch (model.Role)
                            {
                                case "Admin":
                                case "Moderator":
                                    AddUserToRoleMod(user, model.Role);
                                    break;
                                case "Author":
                                    AddUserToRoleAuthor(user, model.Role);
                                    break;
                                default:
                                    await userManager.AddToRoleAsync(user, model.Role);
                                    break;
                            }

                            var userModel = mapper.Map<WakuUser, UserModel>(user);

                            return Created($"/account/{userModel.Username}", userModel);
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

        [Authorize(Roles = "Admin")]
        private async void AddUserToRoleMod(WakuUser user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
        }

        [Authorize(Roles = "Admin,Moderator")]
        private async void AddUserToRoleAuthor(WakuUser user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
        }

        private async Task EnsureRolesCreated()
        {
            await CreateRole("Admin");
            await CreateRole("Moderator");
            await CreateRole("Author");
            await CreateRole("User");
        }

        private async Task CreateRole(string roleName)
        {
            if (!(await roleManager.RoleExistsAsync(roleName)))
            {
                var role = new IdentityRole();
                role.Name = roleName;
                await roleManager.CreateAsync(role);
            }
        }
    }
}