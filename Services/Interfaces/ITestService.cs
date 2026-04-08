using System.Collections.Generic;
using ProjectLMS.Models.ViewModels;

namespace ProjectLMS.Services.Interfaces;

public interface ITestService
{
    Task<IEnumerable<TestViewModel>> GetTestsByCourseAsync(int courseId);
    Task<TestViewModel> GetTestByIdAsync(int id);
    Task CreateTestAsync(TestViewModel test);
    Task UpdateTestAsync(TestViewModel test);
    Task DeleteTestAsync(int id);
    Task<TestViewModel> GetTestForStudentAsync(int testId, int studentId);
    Task<(int score, int grade, IEnumerable<StudentAnswerViewModel> answers)> SubmitTestAsync(int testId, int studentId, IReadOnlyDictionary<int, int> selectedAnswerIds);
}
