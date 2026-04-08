using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectLMS.Models.ViewModels;

/// <summary>
/// DTO for lesson display and creation/editing.
/// </summary>
public class LessonViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Lesson title is required.")]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; }

    [StringLength(5000)]
    public string Content { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(1, int.MaxValue)]
    public int Order { get; set; }

    public DateTime CreatedAt { get; set; }
    public string CourseName { get; set; }
}
