using Microsoft.AspNetCore.Mvc;
using ProjectLMS.Models.ViewModels;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Controllers;

/// <summary>
/// Controller for test management and test-taking operations.
/// Handles CRUD for tests and student test attempts.
/// </summary>
public class TestController : Controller
{
    private readonly ITestService _testService;
    private readonly ICourseService _courseService;

    public TestController(ITestService testService, ICourseService courseService)
    {
        _testService = testService;
        _courseService = courseService;
    }

    /// <summary>
    /// GET: /Test/Index/{courseId}
    /// Displays all tests for a specific course.
    /// </summary>
    public async Task<IActionResult> Index(int courseId)
    {
        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null)
            return NotFound();

        var tests = await _testService.GetTestsByCourseAsync(courseId);
        ViewBag.Course = course;
        return View(tests);
    }

    /// <summary>
    /// GET: /Test/Details/{id}
    /// Displays detailed information about a specific test.
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var test = await _testService.GetTestByIdAsync(id);
        if (test == null)
            return NotFound();

        return View(test);
    }

    /// <summary>
    /// GET: /Test/Create/{courseId}
    /// Shows the form to create a new test in a course.
    /// </summary>
    public async Task<IActionResult> Create(int courseId)
    {
        var course = await _courseService.GetCourseByIdAsync(courseId);
        if (course == null)
            return NotFound();

        var testViewModel = new TestViewModel
        {
            CourseId = courseId,
            CourseName = course.Title,
            PassingScore = 50 // Default passing score
        };

        return View(testViewModel);
    }

    /// <summary>
    /// POST: /Test/Create
    /// Creates a new test.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TestViewModel testViewModel)
    {
        if (ModelState.IsValid)
        {
            await _testService.CreateTestAsync(testViewModel);
            return RedirectToAction(nameof(Index), new { courseId = testViewModel.CourseId });
        }

        // Reload course name if validation fails
        var course = await _courseService.GetCourseByIdAsync(testViewModel.CourseId);
        testViewModel.CourseName = course?.Title ?? "Unknown Course";

        return View(testViewModel);
    }

    /// <summary>
    /// GET: /Test/Edit/{id}
    /// Shows the form to edit an existing test.
    /// </summary>
    public async Task<IActionResult> Edit(int id)
    {
        var test = await _testService.GetTestByIdAsync(id);
        if (test == null)
            return NotFound();

        return View(test);
    }

    /// <summary>
    /// POST: /Test/Edit/{id}
    /// Updates an existing test.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TestViewModel testViewModel)
    {
        if (id != testViewModel.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                await _testService.UpdateTestAsync(testViewModel);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index), new { courseId = testViewModel.CourseId });
        }
        return View(testViewModel);
    }

    /// <summary>
    /// GET: /Test/Delete/{id}
    /// Shows confirmation for deleting a test.
    /// </summary>
    public async Task<IActionResult> Delete(int id)
    {
        var test = await _testService.GetTestByIdAsync(id);
        if (test == null)
            return NotFound();

        return View(test);
    }

    /// <summary>
    /// POST: /Test/Delete/{id}
    /// Deletes a test.
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var test = await _testService.GetTestByIdAsync(id);
        if (test == null)
            return NotFound();

        var courseId = test.CourseId;

        try
        {
            await _testService.DeleteTestAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index), new { courseId });
    }

    /// <summary>
    /// GET: /Test/Take/{id}
    /// Shows the test for a student to take.
    /// </summary>
    public async Task<IActionResult> Take(int id)
    {
        try
        {
            // TODO: Get current student ID from authentication
            int studentId = 2; // Temporary: Student user

            var test = await _testService.GetTestForStudentAsync(id, studentId);
            return View(test);
        }
        catch (UnauthorizedAccessException)
        {
            TempData["Error"] = "You are not enrolled in this course.";
            return RedirectToAction("Index", "Course");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// POST: /Test/Submit/{id}
    /// Submits a completed test and shows results.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id, Dictionary<int, int> answers)
    {
        try
        {
            // TODO: Get current student ID from authentication
            int studentId = 2; // Temporary: Student user

            var (score, grade, studentAnswers) = await _testService.SubmitTestAsync(id, studentId, answers);

            var resultViewModel = new TestResultViewModel
            {
                TestId = id,
                Score = (int)Math.Round((double)score), // Convert to int for existing model
                Grade = grade,
                IsPassed = score >= 50, // Default passing score
                AttemptedAt = DateTime.UtcNow
            };

            ViewBag.StudentAnswers = studentAnswers;
            return View("Result", resultViewModel);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            TempData["Error"] = "An error occurred while submitting the test: " + ex.Message;
            return RedirectToAction(nameof(Take), new { id });
        }
    }

    /// <summary>
    /// GET: /Test/Result
    /// Shows the result of a completed test.
    /// </summary>
    public IActionResult Result()
    {
        return View();
    }
}
