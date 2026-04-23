using Microsoft.EntityFrameworkCore;
using ProjectLMS.Data;
using ProjectLMS.Models.Entities;
using ProjectLMS.Models.ViewModels;
using ProjectLMS.Services.Interfaces;

namespace ProjectLMS.Services.Implementations;

/// <summary>
/// Service for managing lessons within courses.
/// Handles lesson CRUD operations and ordering.
/// </summary>
public class LessonService : ILessonService
{
    private readonly ApplicationDbContext _context;

    public LessonService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all lessons for a specific course.
    /// </summary>
    public async Task<IEnumerable<LessonViewModel>> GetLessonsByCourseAsync(int courseId)
    {
        return await _context.Lessons
            .Include(l => l.Course)
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .Select(l => new LessonViewModel
            {
                Id = l.Id,
                Title = l.Title,
                Content = l.Content,
                CourseId = l.CourseId,
                Order = l.Order,
                CreatedAt = l.CreatedAt,
                CourseName = l.Course.Title
            })
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific lesson by ID.
    /// </summary>
    public async Task<LessonViewModel> GetLessonByIdAsync(int id)
    {
        var lesson = await _context.Lessons
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
            throw new InvalidOperationException("Lesson not found.");

        return new LessonViewModel
        {
            Id = lesson.Id,
            Title = lesson.Title,
            Content = lesson.Content,
            CourseId = lesson.CourseId,
            Order = lesson.Order,
            CreatedAt = lesson.CreatedAt,
            CourseName = lesson.Course.Title
        };
    }

    /// <summary>
    /// Creates a new lesson in a course.
    /// </summary>
    public async Task CreateLessonAsync(LessonViewModel lessonViewModel)
    {
        // Get the next order number for the course
        var maxOrder = await _context.Lessons
            .Where(l => l.CourseId == lessonViewModel.CourseId)
            .MaxAsync(l => (int?)l.Order) ?? 0;

        var lesson = new Lesson
        {
            Title = lessonViewModel.Title,
            Content = lessonViewModel.Content,
            CourseId = lessonViewModel.CourseId,
            Order = lessonViewModel.Order > 0 ? lessonViewModel.Order : maxOrder + 1
        };

        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing lesson.
    /// </summary>
    public async Task UpdateLessonAsync(LessonViewModel lessonViewModel)
    {
        var lesson = await _context.Lessons.FindAsync(lessonViewModel.Id);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        lesson.Title = lessonViewModel.Title;
        lesson.Content = lessonViewModel.Content;
        lesson.Order = lessonViewModel.Order;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a lesson.
    /// </summary>
    public async Task DeleteLessonAsync(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson == null)
            throw new KeyNotFoundException("Lesson not found");

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
    }
}
