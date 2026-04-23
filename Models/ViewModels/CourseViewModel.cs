using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectLMS.Models.ViewModels;

/// <summary>
/// DTO for displaying course information.
/// Prevents direct exposure of database entity.
/// </summary>
public class CourseViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Course title is required.")]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    public string CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public int LessonCount { get; set; }
    public int EnrolledStudentCount { get; set; }
}
