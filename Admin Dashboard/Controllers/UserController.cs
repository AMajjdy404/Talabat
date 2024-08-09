using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities.Identity;

namespace Admin_Dashboard.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            var MappedUsers = users.Select(u => new UserViewModel()
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                Username = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Roles = userManager.GetRolesAsync(u).Result
            }).ToList();
            return View(MappedUsers);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user =  await userManager.FindByIdAsync(id);
            var AllRoles = await roleManager.Roles.ToListAsync();
            var UserRoleVM = new UserRoleViewModel()
            {
                Id=user.Id,
                UserName=user.UserName,
                Roles = AllRoles.Select(r => new RoleViewModel()
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsSelected = userManager.IsInRoleAsync(user,r.Name).Result
                }).ToList()
            };
            return View(UserRoleVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserRoleViewModel userRoleVM)
        {
            var user = await userManager.FindByIdAsync(userRoleVM.Id);
            var userRoles = await userManager.GetRolesAsync(user);

            foreach(var role in userRoleVM.Roles)
            {
                if(userRoles.Any(r => r == role.Name) && !role.IsSelected)
                    await userManager.RemoveFromRoleAsync(user,role.Name);

                if (!userRoles.Any(r => r == role.Name) && role.IsSelected)
                    await userManager.AddToRoleAsync(user, role.Name);
            }
            return RedirectToAction("Index");
        }
    }
}
