using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XYZ_University.Models
{
    public class Office
    {
        [Key]
        public int OfficeId { get; set; }

        [Required]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        public string? InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public ApplicationUser? Instructor { get; set; }
    }
}