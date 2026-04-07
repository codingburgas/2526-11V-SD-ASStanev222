using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Represents a test/quiz in a course.
/// Contains multiple questions with automatic grading.
/// Used for student assessment and progress tracking.
/// </summary>
public class Test : BaseEntity
{
    [Required(ErrorMessage = "Test title is required.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters.")]
    public string Title { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string Description { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(1, 100, ErrorMessage = "Passing score must be between 1 and 100.")]
    public int PassingScore { get; set; } = 50;

    // Foreign key reference
    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    // Navigation properties
    public ICollection<TestQuestion> Questions { get; set; } = new List<TestQuestion>();
    public ICollection<StudentTestAttempt> StudentAttempts { get; set; } = new List<StudentTestAttempt>();
}
