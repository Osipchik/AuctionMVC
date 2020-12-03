using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Core;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication4.DTO.Admin;
using WebApplication4.EmailSender;

namespace WebApplication4.Controllers
{
    [Authorize(Roles = "superAdmin, admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender<MailMessage> _emailSender;
        private readonly ICategoryRepository _categoryRepository;

        private readonly AppDbContext _context;

        public AdminController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IEmailSender<MailMessage> emailSender,
            ICategoryRepository categoryRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _emailSender = emailSender;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Panel()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.UserId());
            var roles = await _userManager.GetRolesAsync(user);

            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(int take = 30, int skip = 0)
        {
            var users = await (
                from u in _context.Users
                let query = (from ur in _context.UserRoles
                    where ur.UserId.Equals(u.Id)
                    join r in _context.Roles on ur.RoleId equals r.Id
                    select r.Name)
                    .Skip(skip)
                    .Take(take)
                select new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    EmailConfirmed = u.EmailConfirmed,
                    RoleNames = query.ToList()
                }).ToListAsync();


            return PartialView("_UsersViev", users);
        }
        

        [HttpGet]
        public async Task<IActionResult> User(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            
            var userRoles = await _userManager.GetRolesAsync(user);
            var roles = await _roleManager.Roles.ToListAsync();
            
            // await _context.Entry(user).Collection(i => i.Lots).LoadAsync();
            // user.Lots = user.Lots.OrderBy(i => i.CreatedAt).ToList();
            //
            var userModel = new UserInfo
            {
                User = user,
                UserRoles = userRoles,
                Roles = roles
            };

            return View(userModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
 
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
                
                return RedirectToAction("Panel");
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(string to, string subject, string message)
        {
            var mailMessage = new MailMessage(to, "", message, EmailTypes.Message);
            await _emailSender.Send(mailMessage, subject);
            
            return RedirectToAction("Panel");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategories()
        {
            var categories = await _context.Categories.OrderBy(i => i.Name).ToListAsync();
            ViewBag.Categories = categories;
            
            return View(new EditModels());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategories(EditModels model)
        {
            var categories = await _context.Categories.OrderBy(i => i.Name).ToListAsync();
            
            if (ModelState.IsValid)
            {
                if (categories.All(i => i.Name != model.CategoryName))
                {
                    var category = new Category
                    {
                        Name = model.CategoryName
                    };
                    await _context.Categories.AddAsync(category);
                    await _context.SaveChangesAsync();
                    
                    categories.Add(category);
                }
                else
                {
                    ModelState.AddModelError("CategoryName", "This name is already exist");
                }
            }
            
            ViewBag.Categories = categories;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _categoryRepository.Find(categoryId);
            await _categoryRepository.Delete(category);
            
            return RedirectToAction("EditCategories");
        }
    }
}