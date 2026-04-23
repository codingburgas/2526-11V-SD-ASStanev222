using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Represents a course in the LMS.
/// Contains multiple lessons and tests.
/// Teachers/Admins can create courses.
/// Students enroll in courses through the Enrollment entity.
/// </summary>
public class Course : BaseEntity
{
    [Required(ErrorMessage = "Course title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    public string Title { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; }

    [Required]
    public string CreatedByUserId { get; set; }

    // Navigation properties
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Test> Tests { get; set; } = new List<Test>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
