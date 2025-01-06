using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.Logic.DlServices.Implementations;

public interface ISolutionVerdictLogAccessor
{
    Task<SolutionVerdict?> GetTaskVerdictAsync(HttpClient client, long courseId, long taskId);
}