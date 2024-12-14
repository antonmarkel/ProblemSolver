using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Interfaces
{
    public interface ITaskTextProcessor
    {
        Task<List<TaskInfo>> ProcessTasksAsync(List<TaskLink> taskLinks, HttpClient client);
    }
}
