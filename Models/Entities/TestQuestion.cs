using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Represents a single question within a test.
/// Supports multiple choice questions with one correct answer.
/// </summary>
public class TestQuestion : BaseEntity
{
    [Required(ErrorMessage = "Question text is required.")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Question must be between 5 and 1000 characters.")]
    public string QuestionText { get; set; }

    [Required]
    public int TestId { get; set; }

    [Range(1, 100, ErrorMessage = "Points must be between 1 and 100.")]
    public int Points { get; set; } = 1;

    // Foreign key reference
    [ForeignKey("TestId")]
    public Test Test { get; set; }

    // Navigation properties
    public ICollection<TestAnswer> Answers { get; set; } = new List<TestAnswer>();
    public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
