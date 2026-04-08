using System.Collections.Generic;
using ProjectLMS.Models.ViewModels;

namespace ProjectLMS.Services.Interfaces;

public interface IStatisticsService
{
    Task<double> GetCourseAverageGradeAsync(int courseId);
    Task<IEnumerable<CourseViewModel>> GetCoursesWithMostEnrollmentsAsync(int top);
    Task<StudentProgressViewModel> GetStudentProgressAsync(int studentId);
}
