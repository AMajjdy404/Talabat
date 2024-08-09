using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTOs;
using Talabat.Core.Entities.Identity;

namespace Admin_Dashboard.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public AdminController(SignInManager<AppUser> signInManager,UserManager<AppUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(loginDto.Email);
                if(user is not null)
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password,false);
                    if (!result.Succeeded || !await userManager.IsInRoleAsync(user,"Admin"))
                    {
                        return View(loginDto);
                    }
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(loginDto);

        }

        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");

        }
    }
}
