using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Interfaces;

/// <summary>
///     Interface for extracting link to task(task id, and course id)
/// </summary>
public interface ITaskLinkExtractor
{
    Task<OneOf<List<TaskLink>, Failed>> ExtractFromCourseAsync(long courseId, HttpClient client);
}