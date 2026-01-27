using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XYZ_University.Data;
using XYZ_University.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace XYZ_University.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- DELETE THE OLD INDEX METHOD HERE --- 
        // (I have removed it for you in this code block)

        // GET: Enrollments?courseId=5
        // This SINGLE method now handles both "All List" and "Filtered List"
        public async Task<IActionResult> Index(int? courseId)
        {
            // 1. Start query
            var enrollmentsQuery = _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .AsQueryable();

            // 2. Filter by Course if ID is provided
            if (courseId.HasValue)
            {
                enrollmentsQuery = enrollmentsQuery.Where(e => e.CourseId == courseId);

                var course = await _context.Courses.FindAsync(courseId);
                ViewBag.CourseTitle = course?.Title;
                ViewBag.CourseCode = "CS" + courseId;
                ViewBag.CourseId = courseId;
            }
            else
            {
                ViewBag.CourseTitle = "All Enrollments";
            }

            return View(await enrollmentsQuery.ToListAsync());
        }

        [Authorize(Roles = "Admin,Instructor")]
        // GET: Enrollments/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        [Authorize(Roles = "Admin,Instructor")]
        // POST: Enrollments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,StudentId,Grade")] Enrollment enrollment)
        {
            bool exists = await _context.Enrollments.AnyAsync(e =>
                e.CourseId == enrollment.CourseId &&
                e.StudentId == enrollment.StudentId);

            if (exists)
            {
                ModelState.AddModelError("", "This student is already enrolled in this course.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
                // Redirect back to the specific course list if possible, otherwise main index
                return RedirectToAction(nameof(Index), new { courseId = enrollment.CourseId });
            }

            PopulateDropdowns(enrollment.CourseId, enrollment.StudentId);
            return View(enrollment);
        }

        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Delete(int? courseId, string studentId)
        {
            if (courseId == null || studentId == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.CourseId == courseId && m.StudentId == studentId);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int courseId, string studentId)
        {
            var enrollment = await _context.Enrollments.FindAsync(courseId, studentId);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { courseId = courseId });
        }

        private void PopulateDropdowns(object selectedCourse = null, object selectedStudent = null)
        {
            var coursesQuery = _context.Courses.AsQueryable();
            if (User.IsInRole("Instructor"))
            {
                var userId = _userManager.GetUserId(User);
                coursesQuery = coursesQuery.Where(c => c.InstructorId == userId);
            }
            ViewData["CourseId"] = new SelectList(coursesQuery, "CourseId", "Title", selectedCourse);

            var students = _context.Users.Select(u => new {
                Id = u.Id,
                FullName = (u.FirstName ?? "Unknown") + " " + (u.LastName ?? "") + " (" + (u.Email ?? "No Email") + ")"
            }).ToList();

            ViewData["StudentId"] = new SelectList(students, "Id", "FullName", selectedStudent);
        }

        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Edit(int? courseId, string studentId)
        {
            if (courseId == null || studentId == null) return NotFound();

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null) return NotFound();

            return View(enrollment);
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int courseId, string studentId, [Bind("CourseId,StudentId,Grade")] Enrollment enrollment)
        {
            if (courseId != enrollment.CourseId || studentId != enrollment.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Enrollments.Any(e => e.CourseId == courseId && e.StudentId == studentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { courseId = courseId });
            }
            return View(enrollment);
        }

        // GET: Enrollments/MyGrades
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyGrades()
        {
            var currentUserId = _userManager.GetUserId(User);

            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == currentUserId && !string.IsNullOrEmpty(e.Grade)) // Only graded courses
                .ToListAsync();

            // --- GPA CALCULATION LOGIC ---
            double totalGradePoints = 0;
            int totalCredits = 0;

            foreach (var item in enrollments)
            {
                // 1. Parse the string grade (e.g. "95") to a number
                if (int.TryParse(item.Grade, out int numericGrade))
                {
                    // 2. Convert 0-100 to 4.0 Scale
                    double gradeScale = 0.0;
                    if (numericGrade >= 90) gradeScale = 4.0;       // A
                    else if (numericGrade >= 80) gradeScale = 3.0;  // B
                    else if (numericGrade >= 70) gradeScale = 2.0;  // C
                    else if (numericGrade >= 60) gradeScale = 1.0;  // D
                    else gradeScale = 0.0;                          // F

                    // 3. Calculate Points for this class (Grade * Credits)
                    totalGradePoints += (gradeScale * item.Course.Credits);
                    totalCredits += item.Course.Credits;
                }
            }

            // 4. Final GPA (Avoid division by zero)
            double gpa = totalCredits > 0 ? Math.Round(totalGradePoints / totalCredits, 2) : 0.0;

            // Pass data to View
            ViewBag.GPA = gpa;
            ViewBag.TotalCredits = totalCredits;

            return View(enrollments);
        }

        // GET: Enrollments/MySchedule
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MySchedule()
        {
            var currentUserId = _userManager.GetUserId(User);

            var activeEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Course.Instructor)
                .Where(e => e.StudentId == currentUserId && string.IsNullOrEmpty(e.Grade)) // Filter: No Grade yet
                .ToListAsync();

            return View(activeEnrollments);
        }
    }
}