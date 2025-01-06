using System.Collections.Concurrent;
using System.Collections.Immutable;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.Shared.Tasks.Enums;

namespace ProblemSolver.Logic.SolverServices.Interfaces
{
    public interface ISolver
    {
        Task<ConcurrentDictionary<long, TaskState>> SolveAsync(ImmutableList<TaskInfo> tasks);
    }
}
