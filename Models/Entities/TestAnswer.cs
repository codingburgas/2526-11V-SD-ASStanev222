using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Represents an answer option for a test question.
/// Multiple answers per question, one marked as correct.
/// </summary>
public class TestAnswer : BaseEntity
{
    [Required(ErrorMessage = "Answer text is required.")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Answer must be between 1 and 500 characters.")]
    public string AnswerText { get; set; }

    [Required]
    public int QuestionId { get; set; }

    public bool IsCorrect { get; set; }

    // Foreign key reference
    [ForeignKey("QuestionId")]
    public TestQuestion Question { get; set; }
}
