using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ITaskLinkExtractor
{
    Task<OneOf<List<TaskLink>, Failed>> ExtractFromCourseAsync(long courseId, HttpClient client);
}