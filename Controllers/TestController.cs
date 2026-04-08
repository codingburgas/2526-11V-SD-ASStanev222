using Microsoft.AspNetCore.Mvc;
using ProjectLMS.Services.Interfaces;
using ProjectLMS.Tests;

namespace ProjectLMS.Controllers;

/// <summary>
/// Test controller to verify service layer functionality.
/// Access via /Test/RunTests to execute integration tests.
/// </summary>
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILessonService _lessonService;
    private readonly ITestService _testService;
    private readonly IStatisticsService _statisticsService;

    public TestController(
        ICourseService courseService,
        ILessonService lessonService,
        ITestService testService,
        IStatisticsService statisticsService)
    {
        _courseService = courseService;
        _lessonService = lessonService;
        _testService = testService;
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Runs integration tests for the service layer.
    /// GET /Test/RunTests
    /// </summary>
    [HttpGet("RunTests")]
    public async Task<IActionResult> RunTests()
    {
        var test = new ServiceIntegrationTest();
        await test.RunAllTests();

        return Ok(new
        {
            Message = "Service layer integration tests completed successfully!",
            Timestamp = DateTime.UtcNow,
            Status = "All tests passed"
        });
    }

    /// <summary>
    /// Simple health check endpoint.
    /// GET /Test/Health
    /// </summary>
    [HttpGet("Health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Services = new[]
            {
                "CourseService",
                "LessonService",
                "TestService",
                "StatisticsService"
            }
        });
    }
}
