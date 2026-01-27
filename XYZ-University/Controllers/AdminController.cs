using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XYZ_University.Data;
using XYZ_University.Models;

namespace XYZ_University.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // 1. List All Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Include(u => u.Department) 
                .ToListAsync();
            return View(users);
        }

        // 2. Assign Role (GET)
        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            ViewBag.CurrentRole = userRoles.FirstOrDefault();
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", user.DepartmentId);

            return View(user);
        }

        // Assign Role (POST)
        // 3. Assign Role (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role, int? departmentId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (role == "Instructor" && departmentId == null)
            {
                ModelState.AddModelError("departmentId", "You must select a Department for Instructors.");

                var userRoles = await _userManager.GetRolesAsync(user);
                ViewBag.CurrentRole = userRoles.FirstOrDefault();
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", user.DepartmentId);

                return View(user);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            if (role == "Instructor")
            {
                user.DepartmentId = departmentId;
            }
            else
            {
                user.DepartmentId = null;
            }
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }
    }
}