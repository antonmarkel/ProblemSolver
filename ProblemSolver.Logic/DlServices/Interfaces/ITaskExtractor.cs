using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Interfaces
{
    public interface ITaskExtractor
    {
        Task<OneOf<List<TaskLink>, Failed>> ExtractAsync(long courseId, HttpClient client);
    }
}
