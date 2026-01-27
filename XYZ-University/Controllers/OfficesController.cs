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
    public class OfficesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Inject UserManager to filter by Roles
        public OfficesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Offices
        public async Task<IActionResult> Index()
        {
            var offices = _context.Offices.Include(o => o.Instructor);
            return View(await offices.ToListAsync());
        }

        // GET: Offices/Create
        public async Task<IActionResult> Create()
        {
            // 1. Get ONLY Users in the "Instructor" Role
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");

            // 2. Format list for Dropdown (Name + Email)
            var instructorList = instructors.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName} ({u.Email})"
            });

            ViewData["InstructorId"] = new SelectList(instructorList, "Id", "FullName");
            return View();
        }

        // POST: Offices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfficeId,Location,InstructorId")] Office office)
        {
            if (ModelState.IsValid)
            {
                _context.Add(office);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Reload dropdown if validation fails
            return await Create();
        }

        // GET: Offices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var office = await _context.Offices.FindAsync(id);
            if (office == null) return NotFound();

            // Load Instructors again for the Edit dropdown
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
            var instructorList = instructors.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName} ({u.Email})"
            });

            ViewData["InstructorId"] = new SelectList(instructorList, "Id", "FullName", office.InstructorId);
            return View(office);
        }

        // POST: Offices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OfficeId,Location,InstructorId")] Office office)
        {
            if (id != office.OfficeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(office);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Offices.Any(e => e.OfficeId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return await Create();
        }

        // GET: Offices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var office = await _context.Offices
                .Include(o => o.Instructor)
                .FirstOrDefaultAsync(m => m.OfficeId == id);
            if (office == null) return NotFound();

            return View(office);
        }

        // POST: Offices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var office = await _context.Offices.FindAsync(id);
            if (office != null) _context.Offices.Remove(office);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}