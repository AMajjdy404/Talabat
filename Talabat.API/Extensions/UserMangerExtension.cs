using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.API.Extensions
{
    public static class UserMangerExtension
    {
        public static async Task<AppUser> FindUserWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
        {
            var email = user.FindFirstValue(ClaimTypes.Email);
            var User = await userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email);
            return User;
        }
    }
}
