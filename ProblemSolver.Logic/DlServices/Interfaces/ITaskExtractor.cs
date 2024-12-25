using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ITaskExtractor
{
    Task<OneOf<List<TaskInfo>, Failed>> ExtractTasksAsync(long courseId, HttpClient client);
    Task<TaskInfo> ExtractTaskAsync(TaskLink link, HttpClient client);
}

