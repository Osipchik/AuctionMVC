using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace Web.Controllers
{
    [Authorize(Roles = "superAdmin, admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Panel()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.UserId());
            var roles = await _userManager.GetRolesAsync(user);
            
            return View(roles);
        }
    }
}