using ProblemSolver.Shared.Tasks;
using ProblemSolver.Shared.Tasks.Enums;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace ProblemSolver.Logic.SolverServices.Interfaces
{
    /// <summary>
    ///     Base interfaces responsible for full solution of tasks
    /// </summary>
    public interface ISolver
    {
        Task<ConcurrentDictionary<long, TaskState>> SolveAsync(ImmutableList<TaskInfo> tasks);
    }
}
