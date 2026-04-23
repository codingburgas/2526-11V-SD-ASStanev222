using System.Collections.Generic;
using ProjectLMS.Models.ViewModels;

namespace ProjectLMS.Services.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseViewModel>> GetAllCoursesAsync();
    Task<CourseViewModel> GetCourseByIdAsync(int id);
    Task CreateCourseAsync(CourseViewModel course);
    Task UpdateCourseAsync(CourseViewModel course);
    Task DeleteCourseAsync(int id);
    Task EnrollStudentAsync(int courseId, string userId);
    Task<IEnumerable<CourseViewModel>> GetPopularCoursesAsync(int top);
}
