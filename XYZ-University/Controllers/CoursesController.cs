using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XYZ_University.Data;
using XYZ_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace XYZ_University.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor);
            return View(await courses.ToListAsync());
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            await PopulateInstructorsDropDownList();
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Course course, string CourseType, string LabSoftware, string Platform)
        {
            ModelState.Remove("Department");
            ModelState.Remove("Instructor");
            ModelState.Remove("Enrollments");

            ModelState.Remove("LabSoftware");
            ModelState.Remove("Platform");

            if (CourseType == "Lab" && string.IsNullOrEmpty(LabSoftware))
            {
                ModelState.AddModelError("LabSoftware", "Lab Software is required for Labs.");
            }
            if (CourseType == "Online" && string.IsNullOrEmpty(Platform))
            {
                ModelState.AddModelError("Platform", "Platform is required for Online courses.");
            }

            if (ModelState.IsValid)
            {
                Course newCourse;

                if (CourseType == "Lab")
                {
                    newCourse = new Lab
                    {
                        Title = course.Title,
                        Credits = course.Credits,
                        DepartmentId = course.DepartmentId,
                        InstructorId = course.InstructorId,
                        LabSoftware = LabSoftware
                    };
                }
                else if (CourseType == "Online")
                {
                    newCourse = new OnlineCourse
                    {
                        Title = course.Title,
                        Credits = course.Credits,
                        DepartmentId = course.DepartmentId,
                        InstructorId = course.InstructorId,
                        Platform = Platform
                    };
                }
                else
                {
                    newCourse = course;
                }

                _context.Add(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", course.DepartmentId);
            await PopulateInstructorsDropDownList(course.InstructorId);

            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", course.DepartmentId);
            await PopulateInstructorsDropDownList(course.InstructorId);
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Course course, string LabSoftware, string Platform)
        {
            if (id != course.CourseId) return NotFound();

            var existingCourse = await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (existingCourse == null) return NotFound();

            ModelState.Remove("Department");
            ModelState.Remove("Instructor");
            ModelState.Remove("Enrollments");
            ModelState.Remove("LabSoftware");
            ModelState.Remove("Platform");

            if (existingCourse is Lab && string.IsNullOrEmpty(LabSoftware))
            {
                ModelState.AddModelError("LabSoftware", "Lab Software is required.");
            }
            else if (existingCourse is OnlineCourse && string.IsNullOrEmpty(Platform))
            {
                ModelState.AddModelError("Platform", "Platform is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Course courseToUpdate;

                    if (existingCourse is Lab)
                    {
                        courseToUpdate = new Lab
                        {
                            CourseId = id,
                            Title = course.Title,
                            Credits = course.Credits,
                            DepartmentId = course.DepartmentId,
                            InstructorId = course.InstructorId,
                            LabSoftware = LabSoftware
                        };
                    }
                    else if (existingCourse is OnlineCourse)
                    {
                        courseToUpdate = new OnlineCourse
                        {
                            CourseId = id,
                            Title = course.Title,
                            Credits = course.Credits,
                            DepartmentId = course.DepartmentId,
                            InstructorId = course.InstructorId,
                            Platform = Platform
                        };
                    }
                    else
                    {
                        courseToUpdate = course; 
                    }
                    _context.Update(courseToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Courses.Any(e => e.CourseId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", course.DepartmentId);
            await PopulateInstructorsDropDownList(course.InstructorId);
            return View(course);
        }

        // Helper Method used by Create and Edit
        private async Task PopulateInstructorsDropDownList(object selectedInstructor = null)
        {
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
            var instructorList = instructors.Select(u => new
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName} ({u.Email})"
            });

            ViewData["InstructorId"] = new SelectList(instructorList, "Id", "FullName", selectedInstructor);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null) return NotFound();
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null) return NotFound();
            return View(course);
        }

        // POST: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null) _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/MyCourses
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> MyCourses()
        {
            var userId = _userManager.GetUserId(User);

            var courses = await _context.Courses
                .Include(c => c.Enrollments)
                .Where(c => c.InstructorId == userId)
                .ToListAsync();

            return View(courses);
        }


        // Endpoint for Cascading Dropdown
        [HttpGet]
        public async Task<JsonResult> GetInstructorsByDepartment(int departmentId, string? selectedInstructorId = null)
        {
            var allInstructors = await _userManager.GetUsersInRoleAsync("Instructor");

            var filteredInstructors = allInstructors
                .Where(u => u.DepartmentId == departmentId || (selectedInstructorId != null && u.Id == selectedInstructorId))
                .Select(u => new
                {
                    id = u.Id,
                    fullName = $"{u.FirstName} {u.LastName} ({u.Email})"
                })
                .Distinct()
                .OrderBy(u => u.fullName)
                .ToList();

            return Json(filteredInstructors);
        }
    }
}