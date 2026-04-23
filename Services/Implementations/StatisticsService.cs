using Microsoft.EntityFrameworkCore;
using ProjectLMS.Data;
using ProjectLMS.Models.ViewModels;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Services.Implementations;

/// <summary>
/// Service for generating statistics and analytics.
/// Uses LINQ queries to calculate averages, rankings, and progress metrics.
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _context;

    public StatisticsService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Calculates the average grade for all students in a course.
    /// </summary>
    public async Task<double> GetCourseAverageGradeAsync(int courseId)
    {
        var testAttempts = await _context.StudentTestAttempts
            .Include(sta => sta.Test)
            .Where(sta => sta.Test.CourseId == courseId)
            .ToListAsync();

        if (!testAttempts.Any())
            return 0.0;

        // Convert scores to grades and calculate average
        var grades = testAttempts.Select(sta => sta.ConvertToGrade()).ToList();
        return grades.Average();
    }

    /// <summary>
    /// Gets courses ranked by enrollment count (most popular first).
    /// </summary>
    public async Task<IEnumerable<CourseViewModel>> GetCoursesWithMostEnrollmentsAsync(int top)
    {
        return await _context.Courses
            .Include(c => c.Enrollments)
            .Include(c => c.Lessons)
            .OrderByDescending(c => c.Enrollments.Count)
            .Take(top)
            .Select(c => new CourseViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                CreatedByUserId = c.CreatedByUserId,
                CreatedAt = c.CreatedAt,
                LessonCount = c.Lessons.Count,
                EnrolledStudentCount = c.Enrollments.Count
            })
            .ToListAsync();
    }

    /// <summary>
    /// Gets comprehensive progress information for a student.
    /// </summary>
    public async Task<StudentProgressViewModel> GetStudentProgressAsync(string userId)
    {
        // Get all enrollments for the student
        var enrollments = await _context.Enrollments
            .Include(e => e.Course)
                .ThenInclude(c => c.Lessons)
            .Include(e => e.Course)
                .ThenInclude(c => c.Tests)
            .Where(e => e.UserId == userId)
            .ToListAsync();

        var courseProgressList = new List<CourseProgressViewModel>();

        foreach (var enrollment in enrollments)
        {
            var course = enrollment.Course;

            // Get test results for this course
            var testAttempts = await _context.StudentTestAttempts
                .Include(sta => sta.Test)
                .Where(sta => sta.UserId == userId && sta.Test.CourseId == course.Id)
                .ToListAsync();

            var testResults = testAttempts.Select(sta => new TestResultViewModel
            {
                TestAttemptId = sta.Id,
                TestId = sta.TestId,
                TestName = sta.Test.Title,
                Score = sta.Score,
                Grade = sta.ConvertToGrade(),
                IsPassed = sta.Score >= sta.Test.PassingScore,
                AttemptedAt = sta.CreatedAt
            }).ToList();

            var courseProgress = new CourseProgressViewModel
            {
                CourseId = course.Id,
                CourseName = course.Title,
                EnrolledAt = enrollment.CreatedAt,
                TotalLessons = course.Lessons.Count,
                CompletedLessons = 0, // TODO: Implement lesson completion tracking
                TotalTests = course.Tests.Count,
                CompletedTests = testAttempts.Count,
                AverageGrade = testAttempts.Any() ? testAttempts.Average(sta => sta.ConvertToGrade()) : 0,
                TestResults = testResults
            };

            courseProgressList.Add(courseProgress);
        }

        // Calculate overall statistics
        var allTestAttempts = await _context.StudentTestAttempts
            .Where(sta => sta.UserId == userId)
            .ToListAsync();

        var overallAverageGrade = allTestAttempts.Any() ? allTestAttempts.Average(sta => sta.ConvertToGrade()) : 0;

        return new StudentProgressViewModel
        {
            UserId = userId,
            StudentName = "Student", // TODO: Get from Identity
            Email = "student@lms.com", // TODO: Get from Identity
            EnrolledCoursesCount = enrollments.Count,
            CourseProgress = courseProgressList,
            OverallAverageGrade = overallAverageGrade,
            CompletedLessonsCount = courseProgressList.Sum(cp => cp.CompletedLessons),
            CompletedTestsCount = allTestAttempts.Count
        };
    }
}
