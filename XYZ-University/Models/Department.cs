namespace XYZ_University.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Code{ get; set; }

        public ICollection<Course>? Courses { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
    }
}
