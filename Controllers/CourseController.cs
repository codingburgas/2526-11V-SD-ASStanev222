using Microsoft.AspNetCore.Mvc;
using ProjectLMS.Models.ViewModels;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Controllers;

/// <summary>
/// Controller for course management operations.
/// Handles CRUD operations for courses, enrollment, and course listing.
/// </summary>
public class CourseController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IStatisticsService _statisticsService;

    public CourseController(ICourseService courseService, IStatisticsService statisticsService)
    {
        _courseService = courseService;
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// GET: /Course/Index
    /// Displays all available courses.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return View(courses);
    }

    /// <summary>
    /// GET: /Course/Details/{id}
    /// Displays detailed information about a specific course.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
            return NotFound();

        return View(course);
    }

    /// <summary>
    /// GET: /Course/Create
    /// Shows the form to create a new course.
    /// </summary>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// POST: /Course/Create
    /// Creates a new course.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseViewModel courseViewModel)
    {
        if (ModelState.IsValid)
        {
            // TODO: Get current user ID from authentication
            courseViewModel.CreatedByUserId = 1; // Temporary: Admin user

            await _courseService.CreateCourseAsync(courseViewModel);
            return RedirectToAction(nameof(Index));
        }
        return View(courseViewModel);
    }

    /// <summary>
    /// GET: /Course/Edit/{id}
    /// Shows the form to edit an existing course.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
            return NotFound();

        return View(course);
    }

    /// <summary>
    /// POST: /Course/Edit/{id}
    /// Updates an existing course.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseViewModel courseViewModel)
    {
        if (id != courseViewModel.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await _courseService.UpdateCourseAsync(courseViewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        return View(courseViewModel);
    }

    /// <summary>
    /// GET: /Course/Delete/{id}
    /// Shows confirmation for deleting a course.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);
        if (course == null)
            return NotFound();

        return View(course);
    }

    /// <summary>
    /// POST: /Course/Delete/{id}
    /// Deletes a course.
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _courseService.DeleteCourseAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// GET: /Course/Popular
    /// Shows the most popular courses by enrollment.
    /// </summary>
    public async Task<IActionResult> Popular(int top = 10)
    {
        var courses = await _courseService.GetPopularCoursesAsync(top);
        ViewBag.TopCount = top;
        return View(courses);
    }

    /// <summary>
    /// POST: /Course/Enroll/{courseId}
    /// Enrolls the current student in a course.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int courseId)
    {
        try
        {
            // TODO: Get current student ID from authentication
            int studentId = 2; // Temporary: Student user
            await _courseService.EnrollStudentAsync(courseId, studentId);
            TempData["Success"] = "Successfully enrolled in the course!";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = courseId });
    }
}
