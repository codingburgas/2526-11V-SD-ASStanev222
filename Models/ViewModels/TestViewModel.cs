using System.ComponentModel.DataAnnotations;

namespace ProjectLMS.Models.ViewModels;

/// <summary>
/// DTO for test display, creation, and editing.
/// </summary>
public class TestViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Test title is required.")]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(1, 100)]
    public int PassingScore { get; set; } = 50;

    public DateTime CreatedAt { get; set; }
    public string CourseName { get; set; }

    // For test-taking
    public List<TestQuestionViewModel> Questions { get; set; } = new();
}

/// <summary>
/// DTO for test questions within a test.
/// </summary>
public class TestQuestionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Question text is required.")]
    [StringLength(1000, MinimumLength = 5)]
    public string QuestionText { get; set; }

    [Range(1, 100)]
    public int Points { get; set; } = 1;

    public int TestId { get; set; }

    // For test-taking
    public List<TestAnswerViewModel> Answers { get; set; } = new();
}

/// <summary>
/// DTO for test answer options.
/// </summary>
public class TestAnswerViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Answer text is required.")]
    [StringLength(500, MinimumLength = 1)]
    public string AnswerText { get; set; }

    public int QuestionId { get; set; }

    // IsCorrect is NOT included here during test-taking (to prevent cheating)
    // It will only be visible after grading in StudentAnswerViewModel
}

/// <summary>
/// DTO for displaying student's answer to a question after grading.
/// </summary>
public class StudentAnswerViewModel
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; }
    public int SelectedAnswerId { get; set; }
    public string SelectedAnswerText { get; set; }
    public bool IsCorrect { get; set; }
    public int Points { get; set; }
}
