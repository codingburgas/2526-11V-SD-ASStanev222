using Microsoft.AspNetCore.Mvc;
using ProjectLMS.Models.ViewModels;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Controllers;

/// <summary>
/// Controller for lesson management operations.
/// Handles CRUD operations for lessons within courses.
/// </summary>
public class LessonController : Controller
{
    private readonly ILessonService _lessonService;
    private readonly ICourseService _courseService;

    public LessonController(ILessonService lessonService, ICourseService courseService)
    {
        _lessonService = lessonService;
        _courseService = courseService;
    }

    /// <summary>
    /// GET: /Lesson/Index/{courseId}
    /// Displays all lessons for a specific course.
    /// </summary>
    public async Task<IActionResult> Index(int courseId)
    {
        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null)
            return NotFound();

        var lessons = await _lessonService.GetLessonsByCourseAsync(courseId);
        ViewBag.Course = course;
        return View(lessons);
    }

    /// <summary>
    /// GET: /Lesson/Details/{id}
    /// Displays detailed information about a specific lesson.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
            return NotFound();

        return View(lesson);
    }

    /// <summary>
    /// GET: /Lesson/Create/{courseId}
    /// Shows the form to create a new lesson in a course.
    /// </summary>
    public async Task<IActionResult> Create(int courseId)
    {
        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null)
            return NotFound();

        var lessonViewModel = new LessonViewModel
        {
            CourseId = courseId,
            CourseName = course.Title
        };

        return View(lessonViewModel);
    }

    /// <summary>
    /// POST: /Lesson/Create
    /// Creates a new lesson.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LessonViewModel lessonViewModel)
    {
        if (ModelState.IsValid)
        {
            await _lessonService.CreateLessonAsync(lessonViewModel);
            return RedirectToAction(nameof(Index), new { courseId = lessonViewModel.CourseId });
        }

        // Reload course name if validation fails
        var course = await _courseService.GetCourseByIdAsync(lessonViewModel.CourseId);
        lessonViewModel.CourseName = course?.Title ?? "Unknown Course";

        return View(lessonViewModel);
    }

    /// <summary>
    /// GET: /Lesson/Edit/{id}
    /// Shows the form to edit an existing lesson.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
            return NotFound();

        return View(lesson);
    }

    /// <summary>
    /// POST: /Lesson/Edit/{id}
    /// Updates an existing lesson.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LessonViewModel lessonViewModel)
    {
        if (id != lessonViewModel.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await _lessonService.UpdateLessonAsync(lessonViewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index), new { courseId = lessonViewModel.CourseId });
        }
        return View(lessonViewModel);
    }

    /// <summary>
    /// GET: /Lesson/Delete/{id}
    /// Shows confirmation for deleting a lesson.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
            return NotFound();

        return View(lesson);
    }

    /// <summary>
    /// POST: /Lesson/Delete/{id}
    /// Deletes a lesson.
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);
        if (lesson == null)
            return NotFound();

        var courseId = lesson.CourseId;

        try
        {
            await _lessonService.DeleteLessonAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), new { courseId });
    }
}
