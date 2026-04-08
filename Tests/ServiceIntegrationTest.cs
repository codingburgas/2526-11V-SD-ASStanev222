using Microsoft.EntityFrameworkCore;
using ProjectLMS.Data;
using ProjectLMS.Models.Entities;
using ProjectLMS.Models.Enums;
using ProjectLMS.Services.Implementations;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Tests;

/// <summary>
/// Simple integration test to verify service layer functionality.
/// Run this to test that services work correctly with the database.
/// </summary>
public class ServiceIntegrationTest
{
    private readonly ApplicationDbContext _context;
    private readonly ICourseService _courseService;
    private readonly ILessonService _lessonService;
    private readonly ITestService _testService;
    private readonly IStatisticsService _statisticsService;

    public ServiceIntegrationTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ProjectLMSTest;Trusted_Connection=True;MultipleActiveResultSets=true")
            .Options;

        _context = new ApplicationDbContext(options);
        _courseService = new CourseService(_context);
        _lessonService = new LessonService(_context);
        _testService = new TestService(_context);
        _statisticsService = new StatisticsService(_context);
    }

    /// <summary>
    /// Runs all service tests.
    /// Call this method to test the entire service layer.
    /// </summary>
    public async Task RunAllTests()
    {
        Console.WriteLine("🧪 Starting Service Layer Integration Tests...\n");

        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();
            Console.WriteLine("✅ Database created successfully");

            // Test 1: Create sample data
            await CreateSampleData();
            Console.WriteLine("✅ Sample data created");

            // Test 2: Test course operations
            await TestCourseOperations();
            Console.WriteLine("✅ Course operations tested");

            // Test 3: Test lesson operations
            await TestLessonOperations();
            Console.WriteLine("✅ Lesson operations tested");

            // Test 4: Test statistics
            await TestStatistics();
            Console.WriteLine("✅ Statistics tested");

            Console.WriteLine("\n🎉 All service tests passed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private async Task CreateSampleData()
    {
        // Create a teacher
        var teacher = new User
        {
            Name = "Dr. Smith",
            Email = "smith@university.edu",
            Role = UserRole.Teacher
        };
        _context.Users.Add(teacher);

        // Create a student
        var student = new User
        {
            Name = "John Doe",
            Email = "john.doe@student.edu",
            Role = UserRole.Student
        };
        _context.Users.Add(student);

        await _context.SaveChangesAsync();
    }

    private async Task TestCourseOperations()
    {
        // Get all users
        var users = await _context.Users.ToListAsync();
        var teacher = users.First(u => u.Role == UserRole.Teacher);
        var student = users.First(u => u.Role == UserRole.Student);

        // Create a course
        var courseViewModel = new ProjectLMS.Models.ViewModels.CourseViewModel
        {
            Title = "Introduction to ASP.NET Core",
            Description = "Learn the fundamentals of ASP.NET Core MVC",
            CreatedByUserId = teacher.Id
        };

        await _courseService.CreateCourseAsync(courseViewModel);

        // Get the created course
        var courses = await _courseService.GetAllCoursesAsync();
        var createdCourse = courses.First();

        // Test enrollment
        await _courseService.EnrollStudentAsync(createdCourse.Id, student.Id);

        // Verify enrollment
        var updatedCourse = await _courseService.GetCourseByIdAsync(createdCourse.Id);
        if (updatedCourse.EnrolledStudentCount != 1)
            throw new Exception("Enrollment count should be 1");
    }

    private async Task TestLessonOperations()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        var course = courses.First();

        // Create lessons
        var lesson1 = new ProjectLMS.Models.ViewModels.LessonViewModel
        {
            Title = "What is ASP.NET Core?",
            Content = "ASP.NET Core is a cross-platform framework...",
            CourseId = course.Id,
            Order = 1
        };

        var lesson2 = new ProjectLMS.Models.ViewModels.LessonViewModel
        {
            Title = "Setting up the Environment",
            Content = "Install Visual Studio and .NET SDK...",
            CourseId = course.Id,
            Order = 2
        };

        await _lessonService.CreateLessonAsync(lesson1);
        await _lessonService.CreateLessonAsync(lesson2);

        // Verify lessons were created
        var lessons = await _lessonService.GetLessonsByCourseAsync(course.Id);
        if (lessons.Count() != 2)
            throw new Exception("Should have 2 lessons");

        // Verify ordering
        var firstLesson = lessons.First();
        var secondLesson = lessons.Skip(1).First();
        if (firstLesson.Order != 1 || secondLesson.Order != 2)
            throw new Exception("Lesson ordering is incorrect");
    }

    private async Task TestStatistics()
    {
        // Test course average (should be 0 since no tests taken yet)
        var courses = await _courseService.GetAllCoursesAsync();
        var course = courses.First();

        var averageGrade = await _statisticsService.GetCourseAverageGradeAsync(course.Id);
        if (averageGrade != 0.0)
            throw new Exception("Average grade should be 0.0 with no tests");

        // Test popular courses
        var popularCourses = await _statisticsService.GetCoursesWithMostEnrollmentsAsync(5);
        if (!popularCourses.Any())
            throw new Exception("Should have at least one course");

        // Test student progress
        var users = await _context.Users.ToListAsync();
        var student = users.First(u => u.Role == UserRole.Student);

        var progress = await _statisticsService.GetStudentProgressAsync(student.Id);
        if (progress.EnrolledCoursesCount != 1)
            throw new Exception("Student should be enrolled in 1 course");
    }
}
