using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Tracks a student's individual answer to a test question.
/// Used for detailed test performance analysis and review.
/// </summary>
public class StudentAnswer : BaseEntity
{
    [Required]
    public int TestAttemptId { get; set; }

    [Required]
    public int QuestionId { get; set; }

    [Required]
    public int SelectedAnswerId { get; set; }

    public bool IsCorrect { get; set; }

    [ForeignKey("TestAttemptId")]
    public StudentTestAttempt TestAttempt { get; set; }

    [ForeignKey("QuestionId")]
    public TestQuestion Question { get; set; }

    [ForeignKey("SelectedAnswerId")]
    public TestAnswer SelectedAnswer { get; set; }
}
