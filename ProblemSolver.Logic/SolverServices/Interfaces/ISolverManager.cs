using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.DL.Models;
using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.Logic.SolverServices.Interfaces
{
    /// <summary>
    ///     For managing solvers
    /// </summary>
    public interface ISolverManager
    {
        Task<OneOf<Success, Failed>> AddSolverAccountAsync(SolverSettings settings, HttpClient client);
        Task<List<SolverAccount>> GetAllSolversAsync();
    }
}
