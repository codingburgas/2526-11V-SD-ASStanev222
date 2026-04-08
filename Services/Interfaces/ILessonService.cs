using System.Collections.Generic;
using ProjectLMS.Models.ViewModels;

namespace ProjectLMS.Services.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonViewModel>> GetLessonsByCourseAsync(int courseId);
    Task<LessonViewModel> GetLessonByIdAsync(int id);
    Task CreateLessonAsync(LessonViewModel lesson);
    Task UpdateLessonAsync(LessonViewModel lesson);
    Task DeleteLessonAsync(int id);
}
