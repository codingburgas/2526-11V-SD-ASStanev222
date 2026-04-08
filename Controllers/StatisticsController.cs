using Microsoft.AspNetCore.Mvc;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Controllers;

/// <summary>
/// Controller for statistics and reporting operations.
/// Provides various statistical views and reports for the LMS.
/// </summary>
public class StatisticsController : Controller
{
    private readonly IStatisticsService _statisticsService;
    private readonly ICourseService _courseService;

    public StatisticsController(IStatisticsService statisticsService, ICourseService courseService)
    {
        _statisticsService = statisticsService;
        _courseService = courseService;
    }

    /// <summary>
    /// GET: /Statistics/Index
    /// Displays the main statistics dashboard.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // For now, show popular courses as the main dashboard
        var popularCourses = await _statisticsService.GetCoursesWithMostEnrollmentsAsync(5);
        return View(popularCourses);
    }

    /// <summary>
    /// GET: /Statistics/Course/{courseId}
    /// Displays statistics for a specific course.
    /// </summary>
    public async Task<IActionResult> Course(int courseId)
    {
        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null)
            return NotFound();

        var averageGrade = await _statisticsService.GetCourseAverageGradeAsync(courseId);
        ViewBag.Course = course;
        ViewBag.AverageGrade = averageGrade;
        return View();
    }

    /// <summary>
    /// GET: /Statistics/Student/{studentId}
    /// Displays statistics for a specific student.
    /// </summary>
    public async Task<IActionResult> Student(int studentId)
    {
        var studentStats = await _statisticsService.GetStudentProgressAsync(studentId);
        if (studentStats == null)
            return NotFound();

        return View(studentStats);
    }

    /// <summary>
    /// GET: /Statistics/PopularCourses
    /// Shows the most popular courses by enrollment.
    /// </summary>
    public async Task<IActionResult> PopularCourses(int top = 10)
    {
        var popularCourses = await _statisticsService.GetCoursesWithMostEnrollmentsAsync(top);
        ViewBag.TopCount = top;
        return View(popularCourses);
    }
}
