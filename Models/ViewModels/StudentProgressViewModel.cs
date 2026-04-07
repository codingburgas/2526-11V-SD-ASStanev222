using System.ComponentModel.DataAnnotations;

namespace ProjectLMS.Models.ViewModels;

/// <summary>
/// DTO for displaying student progress across courses.
/// Used in statistics and dashboard views.
/// </summary>
public class StudentProgressViewModel
{
    public int UserId { get; set; }
    public string StudentName { get; set; }
    public string Email { get; set; }

    // Enrollment data
    public int EnrolledCoursesCount { get; set; }
    public List<CourseProgressViewModel> CourseProgress { get; set; } = new();

    // Overall statistics
    public double OverallAverageGrade { get; set; }
    public int CompletedLessonsCount { get; set; }
    public int CompletedTestsCount { get; set; }
}

/// <summary>
/// DTO for displaying student's progress in a specific course.
/// </summary>
public class CourseProgressViewModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public DateTime EnrolledAt { get; set; }

    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }

    public int TotalTests { get; set; }
    public int CompletedTests { get; set; }

    public double AverageGrade { get; set; }
    public List<TestResultViewModel> TestResults { get; set; } = new();
}

/// <summary>
/// DTO for displaying a test result (student's attempt).
/// </summary>
public class TestResultViewModel
{
    public int TestAttemptId { get; set; }
    public int TestId { get; set; }
    public string TestName { get; set; }
    public int Score { get; set; }
    public int Grade { get; set; }
    public bool IsPassed { get; set; }
    public DateTime AttemptedAt { get; set; }
}
