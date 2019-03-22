using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Waku.Data
{
    public class WakuSeeder
    {
        private readonly WakuContext context;
        private readonly IConfiguration config;
        private readonly UserManager<IdentityUser> userManager;

        public WakuSeeder(
            WakuContext context,
            IConfiguration config,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.config = config;
            this.userManager = userManager;
        }

        public async Task SeedAsync()
        {
            context.Database.EnsureCreated();

            string username = config["Defaults:User:Username"];
            string password = config["Defaults:User:Password"];

            IdentityUser user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = username
                };

                var result = await userManager.CreateAsync(user, password);
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException($"Failed to create user: {result.Errors.ToString()}");
                }
            }
        }
    }
}
