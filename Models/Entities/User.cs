using ProjectLMS.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Represents a user in the LMS (Student, Teacher, or Admin).
/// Links to courses through Enrollment (many-to-many).
/// Teachers create courses and tests.
/// </summary>
public class User : BaseEntity
{
    [Required(ErrorMessage = "User name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email format is invalid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public UserRole Role { get; set; }

    // Navigation properties
    public ICollection<Course> CreatedCourses { get; set; } = new List<Course>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
