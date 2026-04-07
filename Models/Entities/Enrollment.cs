using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Represents the many-to-many relationship between User (Student) and Course.
/// Tracks when a student enrolled in a course.
/// </summary>
public class Enrollment : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    // Foreign key references
    [ForeignKey("UserId")]
    public User User { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    // Composite unique constraint is configured in DbContext
}
