using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.Logic.DlServices.Implementations;

/// <summary>
///     For checking whether the task is solved or not.
/// </summary>
public interface ISolutionVerdictLogAccessor
{
    Task<SolutionVerdict?> GetTaskVerdictAsync(HttpClient client, long courseId, long taskId);
}