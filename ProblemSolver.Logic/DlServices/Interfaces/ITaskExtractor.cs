using OneOf;
using ProblemSolver.Logic.Results;

namespace ProblemSolver.Logic.DlServices.Interfaces
{
    public interface ITaskExtractor
    {
        Task<OneOf<List<string>, Failed>> ExtractAsync(long courseId, HttpClient client);
    }
}
