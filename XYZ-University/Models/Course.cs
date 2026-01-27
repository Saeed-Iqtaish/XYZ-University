using System.ComponentModel.DataAnnotations;

namespace XYZ_University.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(1, 6, ErrorMessage = "Credits must be between 1 and 6")]
        public int Credits { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? InstructorId { get; set; }
        public ApplicationUser? Instructor { get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }

    public class Lab : Course
    {
        [Required]
        public string LabSoftware { get; set; }
    }

    public class OnlineCourse : Course
    {
        public string Platform { get; set; }
    }
}
