using Microsoft.EntityFrameworkCore;
using ProjectLMS.Data;
using ProjectLMS.Models.Entities;
using ProjectLMS.Models.ViewModels;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Services.Implementations;

/// <summary>
/// Service for managing courses, including CRUD operations and student enrollment.
/// Implements business logic for course management.
/// </summary>
public class CourseService : ICourseService
{
    private readonly ApplicationDbContext _context;

    public CourseService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all courses with their basic information.
    /// </summary>
    public async Task<IEnumerable<CourseViewModel>> GetAllCoursesAsync()
    {
        return await _context.Courses
            .Include(c => c.CreatedByUser)
            .Include(c => c.Enrollments)
            .Include(c => c.Lessons)
            .Select(c => new CourseViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                CreatedByUserId = c.CreatedByUserId,
                CreatedByUserName = c.CreatedByUser.Name,
                CreatedAt = c.CreatedAt,
                LessonCount = c.Lessons.Count,
                EnrolledStudentCount = c.Enrollments.Count
            })
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific course by ID with full details.
    /// </summary>
    public async Task<CourseViewModel> GetCourseByIdAsync(int id)
    {
        var course = await _context.Courses
            .Include(c => c.CreatedByUser)
            .Include(c => c.Enrollments)
            .Include(c => c.Lessons)
            .Include(c => c.Tests)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return null;

        return new CourseViewModel
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            CreatedByUserId = course.CreatedByUserId,
            CreatedByUserName = course.CreatedByUser.Name,
            CreatedAt = course.CreatedAt,
            LessonCount = course.Lessons.Count,
            EnrolledStudentCount = course.Enrollments.Count
        };
    }

    /// <summary>
    /// Creates a new course.
    /// </summary>
    public async Task CreateCourseAsync(CourseViewModel courseViewModel)
    {
        var course = new Course
        {
            Title = courseViewModel.Title,
            Description = courseViewModel.Description,
            CreatedByUserId = courseViewModel.CreatedByUserId
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing course.
    /// </summary>
    public async Task UpdateCourseAsync(CourseViewModel courseViewModel)
    {
        var course = await _context.Courses.FindAsync(courseViewModel.Id);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        course.Title = courseViewModel.Title;
        course.Description = courseViewModel.Description;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a course and all related data.
    /// </summary>
    public async Task DeleteCourseAsync(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            throw new KeyNotFoundException("Course not found");

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Enrolls a student in a course.
    /// </summary>
    public async Task EnrollStudentAsync(int courseId, int studentId)
    {
        // Check if already enrolled
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == studentId);

        if (existingEnrollment != null)
            throw new InvalidOperationException("Student is already enrolled in this course");

        var enrollment = new Enrollment
        {
            CourseId = courseId,
            UserId = studentId
        };

        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the most popular courses by enrollment count.
    /// </summary>
    public async Task<IEnumerable<CourseViewModel>> GetPopularCoursesAsync(int top)
    {
        return await _context.Courses
            .Include(c => c.CreatedByUser)
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
                CreatedByUserName = c.CreatedByUser.Name,
                CreatedAt = c.CreatedAt,
                LessonCount = c.Lessons.Count,
                EnrolledStudentCount = c.Enrollments.Count
            })
            .ToListAsync();
    }
}
