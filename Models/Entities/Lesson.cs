using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Represents a lesson within a course.
/// Contains educational content and materials.
/// Ordered sequentially within a course for structured learning.
/// </summary>
public class Lesson : BaseEntity
{
    [Required(ErrorMessage = "Lesson title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    public string Title { get; set; }

    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters.")]
    public string Content { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Order must be greater than 0.")]
    public int Order { get; set; }

    // Foreign key reference
    [ForeignKey("CourseId")]
    public Course Course { get; set; }
}
