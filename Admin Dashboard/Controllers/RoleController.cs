using Admin_Dashboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin_Dashboard.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }


        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExist = await roleManager.RoleExistsAsync(model.Name);
                if (roleExist)
                {
                    ModelState.AddModelError("Name", "This Role is Already Exist");
                    return RedirectToAction("Index", await roleManager.Roles.ToListAsync());
                }
                await roleManager.CreateAsync(new IdentityRole { Name = model.Name.Trim() });
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", await roleManager.Roles.ToListAsync());

        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            await roleManager.DeleteAsync(role);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            var mappedRole = new RoleViewModel() { Id = role.Id , Name = role.Name};
            return View(mappedRole);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleExist = await roleManager.RoleExistsAsync(model.Name);
                if (roleExist)
                {
                    ModelState.AddModelError("Name", "This Role is Already Exist");
                    return RedirectToAction("Index", await roleManager.Roles.ToListAsync());
                }
                var role =await roleManager.FindByIdAsync(model.Id);
                role.Name = model.Name.Trim();
                await roleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
