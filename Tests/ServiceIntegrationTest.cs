// using Microsoft.EntityFrameworkCore;
// using ProjectLMS.Data;
// using ProjectLMS.Models.Entities;
// using ProjectLMS.Models.Enums;
// using ProjectLMS.Services.Implementations;
// using ProjectLMS.Services.Interfaces;

// namespace ProjectLMS.Tests;

// /// <summary>
// /// Simple integration test to verify service layer functionality.
// /// Run this to test that services work correctly with the database.
// /// NOTE: This test is commented out because it uses the old custom User entity
// /// and is incompatible with ASP.NET Core Identity. It would need to be rewritten
// /// to work with IdentityUser and the new authentication system.
// /// </summary>
// public class ServiceIntegrationTest
// {
//     private readonly ApplicationDbContext _context;
//     private readonly ICourseService _courseService;
//     private readonly ILessonService _lessonService;
//     private readonly ITestService _testService;
//     private readonly IStatisticsService _statisticsService;

//     public ServiceIntegrationTest()
//     {
//         var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//             .UseSqlite("Data Source=ProjectLMSTest.db")
//             .Options;

//         _context = new ApplicationDbContext(options);
//         _courseService = new CourseService(_context);
//         _lessonService = new LessonService(_context);
//         _testService = new TestService(_context);
//         _statisticsService = new StatisticsService(_context);
//     }

//     /// <summary>
//     /// Runs all service tests.
//     /// Call this method to test the entire service layer.
//     /// </summary>
//     public async Task RunAllTests()
//     {
//         Console.WriteLine("🧪 Starting Service Layer Integration Tests...\n");

//         try
//         {
//             // Ensure database is created
//             await _context.Database.EnsureCreatedAsync();
//             Console.WriteLine("✅ Database created successfully");

//             // Test 1: Create sample data
//             await CreateSampleData();
//             Console.WriteLine("✅ Sample data created successfully");

//             // Test 2: Course operations
//             await TestCourseOperations();
//             Console.WriteLine("✅ Course operations tested successfully");

//             // Test 3: Lesson operations
//             await TestLessonOperations();
//             Console.WriteLine("✅ Lesson operations tested successfully");

//             // Test 4: Test operations
//             await TestTestOperations();
//             Console.WriteLine("✅ Test operations tested successfully");

//             // Test 5: Statistics operations
//             await TestStatisticsOperations();
//             Console.WriteLine("✅ Statistics operations tested successfully");

//             Console.WriteLine("\n🎉 All tests passed successfully!");
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"❌ Test failed: {ex.Message}");
//             Console.WriteLine($"Stack trace: {ex.StackTrace}");
//         }
//     }

//     private async Task CreateSampleData()
//     {
//         // Create a teacher
//         var teacher = new User
//         {
//             Name = "Dr. Smith",
//             Email = "smith@university.edu",
//             Role = UserRole.Teacher
//         };
//         _context.Users.Add(teacher);

//         // Create a student
//         var student = new User
//         {
//             Name = "John Doe",
//             Email = "john.doe@student.edu",
//             Role = UserRole.Student
//         };
//         _context.Users.Add(student);

//         await _context.SaveChangesAsync();
//     }

//     private async Task TestCourseOperations()
//     {
//         // Test commented out - using Identity now instead of custom User entity
//         /*
//         // Create a course
//         var course = await _courseService.CreateCourseAsync(new CourseViewModel
//         {
//             Title = "Test Course",
//             Description = "A test course for integration testing",
//             CreatedByUserId = 1 // teacher id
//         });

//         if (course == null)
//             throw new Exception("Failed to create course");

//         // Get all courses
//         var courses = await _courseService.GetAllCoursesAsync();
//         if (!courses.Any())
//             throw new Exception("Should have at least one course");

//         // Get course by id
//         var retrievedCourse = await _courseService.GetCourseByIdAsync(course.Id);
//         if (retrievedCourse == null)
//             throw new Exception("Failed to retrieve course by id");

//         // Update course
//         course.Title = "Updated Test Course";
//         var updatedCourse = await _courseService.UpdateCourseAsync(course);
//         if (updatedCourse.Title != "Updated Test Course")
//             throw new Exception("Failed to update course");

//         // Enroll student
//         await _courseService.EnrollStudentAsync(course.Id, 2); // student id

//         // Get enrolled courses for student
//         var enrolledCourses = await _courseService.GetEnrolledCoursesAsync(2); // student id
//         if (!enrolledCourses.Any())
//             throw new Exception("Student should be enrolled in at least one course");
//         */
//     }

//     private async Task TestLessonOperations()
//     {
//         // Test commented out - using Identity now instead of custom User entity
//         /*
//         // Create a lesson
//         var lesson = await _lessonService.CreateLessonAsync(new LessonViewModel
//         {
//             Title = "Test Lesson",
//             Content = "Lesson content for testing",
//             CourseId = 1
//         });

//         if (lesson == null)
//             throw new Exception("Failed to create lesson");

//         // Get lessons for course
//         var lessons = await _lessonService.GetLessonsByCourseIdAsync(1);
//         if (!lessons.Any())
//             throw new Exception("Should have at least one lesson");

//         // Get lesson by id
//         var retrievedLesson = await _lessonService.GetLessonByIdAsync(lesson.Id);
//         if (retrievedLesson == null)
//             throw new Exception("Failed to retrieve lesson by id");

//         // Update lesson
//         lesson.Title = "Updated Test Lesson";
//         var updatedLesson = await _lessonService.UpdateLessonAsync(lesson);
//         if (updatedLesson.Title != "Updated Test Lesson")
//             throw new Exception("Failed to update lesson");
//         */
//     }

//     private async Task TestTestOperations()
//     {
//         // Test commented out - using Identity now instead of custom User entity
//         /*
//         // Create a test
//         var test = await _testService.CreateTestAsync(new TestViewModel
//         {
//             Title = "Test Quiz",
//             Description = "A test quiz for integration testing",
//             CourseId = 1,
//             Questions = new List<TestQuestionViewModel>
//             {
//                 new TestQuestionViewModel
//                 {
//                     QuestionText = "What is 2+2?",
//                     Answers = new List<TestAnswerViewModel>
//                     {
//                         new TestAnswerViewModel { AnswerText = "3", IsCorrect = false },
//                         new TestAnswerViewModel { AnswerText = "4", IsCorrect = true },
//                         new TestAnswerViewModel { AnswerText = "5", IsCorrect = false }
//                     }
//                 }
//             }
//         });

//         if (test == null)
//             throw new Exception("Failed to create test");

//         // Get tests for course
//         var tests = await _testService.GetTestsByCourseIdAsync(1);
//         if (!tests.Any())
//             throw new Exception("Should have at least one test");

//         // Get test by id
//         var retrievedTest = await _testService.GetTestByIdAsync(test.Id);
//         if (retrievedTest == null)
//             throw new Exception("Failed to retrieve test by id");

//         // Submit test attempt
//         var attempt = await _testService.SubmitTestAttemptAsync(test.Id, 2, new List<StudentAnswerViewModel>
//         {
//             new StudentAnswerViewModel { QuestionId = test.Questions.First().Id, SelectedAnswerId = test.Questions.First().Answers.First(a => a.IsCorrect).Id }
//         });

//         if (attempt == null)
//             throw new Exception("Failed to submit test attempt");
//         */
//     }

//     private async Task TestStatisticsOperations()
//     {
//         // Test commented out - using Identity now instead of custom User entity
//         /*
//         // Get overall statistics
//         var stats = await _statisticsService.GetOverallStatisticsAsync();
//         if (stats.TotalCourses < 1)
//             throw new Exception("Should have at least one course");

//         // Get popular courses
//         var popularCourses = await _statisticsService.GetPopularCoursesAsync();
//         if (!popularCourses.Any())
//             throw new Exception("Should have at least one course");

//         // Test student progress
//         var users = await _context.Users.ToListAsync();
//         var student = users.First(u => u.Role == UserRole.Student);

//         var progress = await _statisticsService.GetStudentProgressAsync(student.Id.ToString());
//         if (progress.EnrolledCoursesCount != 1)
//             throw new Exception("Student should be enrolled in 1 course");
//         */
//     }
// }
