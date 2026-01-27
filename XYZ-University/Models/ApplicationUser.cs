using Microsoft.AspNetCore.Identity;

namespace XYZ_University.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public Office? Office{ get; set; }

        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
