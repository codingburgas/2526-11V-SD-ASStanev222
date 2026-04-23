using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Tracks a student's attempt to complete a test.
/// Stores the score and timestamp for grading and progress tracking.
/// </summary>
public class StudentTestAttempt : BaseEntity
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public int TestId { get; set; }

    [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
    public int Score { get; set; }

    /// <summary>
    /// Converts numeric score (0-100) to grade scale (2-6)
    /// </summary>
    public int ConvertToGrade()
    {
        return Score switch
        {
            >= 90 => 6,
            >= 80 => 5,
            >= 70 => 4,
            >= 60 => 3,
            >= 50 => 2,
            _ => 2  // Fail grade
        };
    }

    [ForeignKey("TestId")]
    public Test Test { get; set; }

    // Navigation property
    public ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
}
