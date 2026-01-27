using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XYZ_University.Models
{
    public class Enrollment
    {
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public string StudentId { get; set; }
        public ApplicationUser? Student { get; set; }

        [StringLength(2)]
        public string? Grade { get; set; }
    }
}