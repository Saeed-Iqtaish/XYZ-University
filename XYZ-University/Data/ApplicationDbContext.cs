using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XYZ_University.Models;

namespace XYZ_University.Data
{
    // FIX 1: We use <ApplicationUser> here so the DB knows which User class to use
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<Lab> Labs { get; set; }
        public DbSet<OnlineCourse> OnlineCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // FIX 2: Explicitly map the Department-User relationship to prevent confusion
            builder.Entity<Department>()
                .HasMany(d => d.Users)
                .WithOne(u => u.Department)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-to-1: User <-> Office
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Office)
                .WithOne(o => o.Instructor)
                .HasForeignKey<Office>(o => o.InstructorId);

            // 1-to-Many: Department <-> Courses
            builder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 1-to-Many: Instructor <-> Courses
            builder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany()
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Enrollment>()
                    .HasKey(e => new { e.CourseId, e.StudentId }); // <--- The Magic Line

            // Relationship: Course -> Enrollments
            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Student -> Enrollments
            builder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}