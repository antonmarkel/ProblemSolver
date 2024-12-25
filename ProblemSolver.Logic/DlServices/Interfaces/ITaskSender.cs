using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ITaskSender
{
    Task SendToCheckAsync(TaskSolution solution, HttpClient client);
}