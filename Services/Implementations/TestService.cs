using Microsoft.EntityFrameworkCore;
using ProjectLMS.Data;
using ProjectLMS.Models.Entities;
using ProjectLMS.Models.ViewModels;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Services.Implementations;

/// <summary>
/// Service for managing tests, questions, and grading.
/// Handles test creation, student attempts, and automatic grading.
/// </summary>
public class TestService : ITestService
{
    private readonly ApplicationDbContext _context;

    public TestService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all tests for a specific course.
    /// </summary>
    public async Task<IEnumerable<TestViewModel>> GetTestsByCourseAsync(int courseId)
    {
        return await _context.Tests
            .Include(t => t.Course)
            .Where(t => t.CourseId == courseId)
            .Select(t => new TestViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CourseId = t.CourseId,
                PassingScore = t.PassingScore,
                CreatedAt = t.CreatedAt,
                CourseName = t.Course.Title
            })
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific test by ID.
    /// </summary>
    public async Task<TestViewModel> GetTestByIdAsync(int id)
    {
        var test = await _context.Tests
            .Include(t => t.Course)
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null)
            throw new InvalidOperationException("Test not found.");

        return new TestViewModel
        {
            Id = test.Id,
            Title = test.Title,
            Description = test.Description,
            CourseId = test.CourseId,
            PassingScore = test.PassingScore,
            CreatedAt = test.CreatedAt,
            CourseName = test.Course.Title,
            Questions = test.Questions.Select(q => new TestQuestionViewModel
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                Points = q.Points,
                TestId = q.TestId,
                Answers = q.Answers.Select(a => new TestAnswerViewModel
                {
                    Id = a.Id,
                    AnswerText = a.AnswerText,
                    QuestionId = a.QuestionId
                }).ToList()
            }).ToList()
        };
    }

    /// <summary>
    /// Creates a new test.
    /// </summary>
    public async Task CreateTestAsync(TestViewModel testViewModel)
    {
        var test = new Test
        {
            Title = testViewModel.Title,
            Description = testViewModel.Description,
            CourseId = testViewModel.CourseId,
            PassingScore = testViewModel.PassingScore
        };

        _context.Tests.Add(test);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing test.
    /// </summary>
    public async Task UpdateTestAsync(TestViewModel testViewModel)
    {
        var test = await _context.Tests.FindAsync(testViewModel.Id);
        if (test == null)
            throw new KeyNotFoundException("Test not found");

        test.Title = testViewModel.Title;
        test.Description = testViewModel.Description;
        test.PassingScore = testViewModel.PassingScore;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a test and all related questions/answers.
    /// </summary>
    public async Task DeleteTestAsync(int id)
    {
        var test = await _context.Tests.FindAsync(id);
        if (test == null)
            throw new KeyNotFoundException("Test not found");

        _context.Tests.Remove(test);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets a test for a student to take (without correct answers).
    /// </summary>
    public async Task<TestViewModel> GetTestForStudentAsync(int testId, string userId)
    {
        var test = await _context.Tests
            .Include(t => t.Course)
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
            throw new InvalidOperationException("Test not found.");

        // Check if student is enrolled in the course
        var isEnrolled = await _context.Enrollments
            .AnyAsync(e => e.CourseId == test.CourseId && e.UserId == userId);

        if (!isEnrolled)
            throw new UnauthorizedAccessException("Student is not enrolled in this course");

        return new TestViewModel
        {
            Id = test.Id,
            Title = test.Title,
            Description = test.Description,
            CourseId = test.CourseId,
            PassingScore = test.PassingScore,
            CreatedAt = test.CreatedAt,
            CourseName = test.Course.Title,
            Questions = test.Questions.Select(q => new TestQuestionViewModel
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                Points = q.Points,
                TestId = q.TestId,
                Answers = q.Answers.Select(a => new TestAnswerViewModel
                {
                    Id = a.Id,
                    AnswerText = a.AnswerText,
                    QuestionId = a.QuestionId
                    // Note: IsCorrect is NOT included for security
                }).ToList()
            }).ToList()
        };
    }

    /// <summary>
    /// Submits a test attempt and calculates the score.
    /// </summary>
    public async Task<(int score, int grade, IEnumerable<StudentAnswerViewModel> answers)> SubmitTestAsync(
        int testId, string userId, IReadOnlyDictionary<int, int> selectedAnswerIds)
    {
        var test = await _context.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == testId);

        if (test == null)
            throw new KeyNotFoundException("Test not found");

        // Create test attempt record
        var attempt = new StudentTestAttempt
        {
            UserId = userId,
            TestId = testId,
            Score = 0 // Will be calculated
        };

        _context.StudentTestAttempts.Add(attempt);
        await _context.SaveChangesAsync(); // Save to get the ID

        var totalScore = 0;
        var maxScore = test.Questions.Sum(q => q.Points);
        var studentAnswers = new List<StudentAnswerViewModel>();

        foreach (var question in test.Questions)
        {
            var selectedAnswerId = selectedAnswerIds.GetValueOrDefault(question.Id, 0);
            var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);
            var isCorrect = selectedAnswer?.IsCorrect ?? false;

            if (isCorrect)
                totalScore += question.Points;

            // Record student's answer
            var studentAnswer = new StudentAnswer
            {
                TestAttemptId = attempt.Id,
                QuestionId = question.Id,
                SelectedAnswerId = selectedAnswerId,
                IsCorrect = isCorrect
            };

            _context.StudentAnswers.Add(studentAnswer);

            studentAnswers.Add(new StudentAnswerViewModel
            {
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                SelectedAnswerId = selectedAnswerId,
                SelectedAnswerText = selectedAnswer?.AnswerText ?? "No answer",
                IsCorrect = isCorrect,
                Points = isCorrect ? question.Points : 0
            });
        }

        // Calculate percentage score
        var percentageScore = maxScore > 0 ? (int)Math.Round((double)totalScore / maxScore * 100) : 0;

        // Update attempt with final score
        attempt.Score = percentageScore;
        await _context.SaveChangesAsync();

        // Convert to grade (2-6 scale)
        var grade = attempt.ConvertToGrade();

        return (percentageScore, grade, studentAnswers);
    }
}
