using ProjectLMS.Data;
using ProjectLMS.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ProjectLMS.Data;

/// <summary>
/// Seeds the database with initial data for development and testing.
/// </summary>
public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        context.Database.EnsureCreated();

        // Create roles if they don't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("Teacher"))
            await roleManager.CreateAsync(new IdentityRole("Teacher"));

        if (!await roleManager.RoleExistsAsync("Student"))
            await roleManager.CreateAsync(new IdentityRole("Student"));

        // Create users if they don't exist
        IdentityUser? adminUser = null;
        IdentityUser? teacherUser = null;
        IdentityUser? studentUser = null;

        if (await userManager.FindByEmailAsync("admin@lms.com") == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin@lms.com",
                Email = "admin@lms.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "password");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else
        {
            adminUser = await userManager.FindByEmailAsync("admin@lms.com");
        }

        if (await userManager.FindByEmailAsync("teacher1@lms.com") == null)
        {
            teacherUser = new IdentityUser
            {
                UserName = "teacher1@lms.com",
                Email = "teacher1@lms.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(teacherUser, "password");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(teacherUser, "Teacher");
            }
        }
        else
        {
            teacherUser = await userManager.FindByEmailAsync("teacher1@lms.com");
        }

        if (await userManager.FindByEmailAsync("student1@lms.com") == null)
        {
            studentUser = new IdentityUser
            {
                UserName = "student1@lms.com",
                Email = "student1@lms.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(studentUser, "password");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }
        else
        {
            studentUser = await userManager.FindByEmailAsync("student1@lms.com");
        }

        // Seed courses if they don't exist
        if (!context.Courses.Any())
        {
            var courses = new[]
            {
                new Course
                {
                    Title = "Introduction to ASP.NET Core",
                    Description = "Learn the fundamentals of ASP.NET Core MVC framework, including controllers, views, and models.",
                    CreatedByUserId = teacherUser?.Id ?? adminUser?.Id ?? "1"
                },
                new Course
                {
                    Title = "Database Design with Entity Framework",
                    Description = "Master Entity Framework Core for database operations, migrations, and data modeling.",
                    CreatedByUserId = teacherUser?.Id ?? adminUser?.Id ?? "1"
                },
                new Course
                {
                    Title = "Web Development Fundamentals",
                    Description = "Essential concepts of modern web development including HTML, CSS, JavaScript, and responsive design.",
                    CreatedByUserId = teacherUser?.Id ?? adminUser?.Id ?? "1"
                },
                new Course
                {
                    Title = "C# Programming Advanced",
                    Description = "Advanced C# programming concepts including LINQ, async/await, and design patterns.",
                    CreatedByUserId = teacherUser?.Id ?? adminUser?.Id ?? "1"
                },
                new Course
                {
                    Title = "Software Architecture Patterns",
                    Description = "Learn about different software architecture patterns and when to use them in your applications.",
                    CreatedByUserId = teacherUser?.Id ?? adminUser?.Id ?? "1"
                }
            };

            context.Courses.AddRange(courses);
            context.SaveChanges();
        }

        // Seed lessons for courses
        if (!context.Lessons.Any())
        {
            var course1 = context.Courses.First(c => c.Title.Contains("ASP.NET"));
            var course2 = context.Courses.First(c => c.Title.Contains("Database"));
            var course3 = context.Courses.First(c => c.Title.Contains("Web Development"));

            var lessons = new[]
            {
                // ASP.NET Course lessons
                new Lesson
                {
                    CourseId = course1.Id,
                    Title = "Getting Started with ASP.NET Core",
                    Content = "Introduction to ASP.NET Core framework and project structure.",
                    Order = 1
                },
                new Lesson
                {
                    CourseId = course1.Id,
                    Title = "MVC Pattern Overview",
                    Content = "Understanding Model-View-Controller architecture in ASP.NET Core.",
                    Order = 2
                },
                new Lesson
                {
                    CourseId = course1.Id,
                    Title = "Controllers and Actions",
                    Content = "Creating controllers and handling HTTP requests with actions.",
                    Order = 3
                },

                // Database Course lessons
                new Lesson
                {
                    CourseId = course2.Id,
                    Title = "Entity Framework Basics",
                    Content = "Introduction to Entity Framework Core and ORM concepts.",
                    Order = 1
                },
                new Lesson
                {
                    CourseId = course2.Id,
                    Title = "Code First Approach",
                    Content = "Creating database schema using Code First migrations.",
                    Order = 2
                },

                // Web Development lessons
                new Lesson
                {
                    CourseId = course3.Id,
                    Title = "HTML5 Fundamentals",
                    Content = "Modern HTML5 elements and semantic markup.",
                    Order = 1
                },
                new Lesson
                {
                    CourseId = course3.Id,
                    Title = "CSS3 and Responsive Design",
                    Content = "Styling web pages with CSS3 and creating responsive layouts.",
                    Order = 2
                }
            };

            context.Lessons.AddRange(lessons);
            context.SaveChanges();
        }

        // Seed tests
        if (!context.Tests.Any())
        {
            var course1 = context.Courses.First(c => c.Title.Contains("ASP.NET"));
            var course2 = context.Courses.First(c => c.Title.Contains("Database"));

            var tests = new[]
            {
                new Test
                {
                    CourseId = course1.Id,
                    Title = "ASP.NET Core Fundamentals Quiz",
                    Description = "Test your knowledge of ASP.NET Core basics",
                    PassingScore = 70
                },
                new Test
                {
                    CourseId = course2.Id,
                    Title = "Entity Framework Assessment",
                    Description = "Evaluate your understanding of Entity Framework Core",
                    PassingScore = 75
                }
            };

            context.Tests.AddRange(tests);
            context.SaveChanges();
        }

        // Seed test questions
        if (!context.TestQuestions.Any())
        {
            var test1 = context.Tests.First(t => t.Title.Contains("ASP.NET"));
            var test2 = context.Tests.First(t => t.Title.Contains("Entity Framework"));

            var questions = new[]
            {
                // ASP.NET Test Questions
                new TestQuestion
                {
                    TestId = test1.Id,
                    QuestionText = "What does MVC stand for in ASP.NET Core?",
                    Points = 10
                },
                new TestQuestion
                {
                    TestId = test1.Id,
                    QuestionText = "Which method is used to register services in ASP.NET Core?",
                    Points = 10
                },

                // Entity Framework Test Questions
                new TestQuestion
                {
                    TestId = test2.Id,
                    QuestionText = "What is the purpose of Entity Framework migrations?",
                    Points = 15
                }
            };

            context.TestQuestions.AddRange(questions);
            context.SaveChanges();

            // Now add answers for each question
            var q1 = context.TestQuestions.First(q => q.QuestionText.Contains("MVC"));
            var q2 = context.TestQuestions.First(q => q.QuestionText.Contains("register services"));
            var q3 = context.TestQuestions.First(q => q.QuestionText.Contains("migrations"));

            var answers = new[]
            {
                // Answers for Q1
                new TestAnswer { QuestionId = q1.Id, AnswerText = "Model View Controller", IsCorrect = true },
                new TestAnswer { QuestionId = q1.Id, AnswerText = "Modern View Component", IsCorrect = false },
                new TestAnswer { QuestionId = q1.Id, AnswerText = "Managed Virtual Code", IsCorrect = false },
                new TestAnswer { QuestionId = q1.Id, AnswerText = "Multi-Version Control", IsCorrect = false },

                // Answers for Q2
                new TestAnswer { QuestionId = q2.Id, AnswerText = "ConfigureServices", IsCorrect = true },
                new TestAnswer { QuestionId = q2.Id, AnswerText = "Configure", IsCorrect = false },
                new TestAnswer { QuestionId = q2.Id, AnswerText = "Startup", IsCorrect = false },
                new TestAnswer { QuestionId = q2.Id, AnswerText = "Main", IsCorrect = false },

                // Answers for Q3
                new TestAnswer { QuestionId = q3.Id, AnswerText = "To update database schema", IsCorrect = true },
                new TestAnswer { QuestionId = q3.Id, AnswerText = "To backup data", IsCorrect = false },
                new TestAnswer { QuestionId = q3.Id, AnswerText = "To optimize queries", IsCorrect = false },
                new TestAnswer { QuestionId = q3.Id, AnswerText = "To create indexes", IsCorrect = false }
            };

            context.TestAnswers.AddRange(answers);
            context.SaveChanges();
        }
    }
}