using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Abdalla Magdy",
                    Email = "abdallamagdy@gmail.com",
                    UserName = "abdallamagdy",
                    PhoneNumber = "01234567891"
                };
                await userManager.CreateAsync(User,"P@ssw0rd");
            }
        }
    }
}
