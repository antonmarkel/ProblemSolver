using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Logic.SolverServices.Interfaces
{
    /// <summary>
    ///     For configuring solver depending on solver config
    /// </summary>
    /// <typeparam name="TSolver"></typeparam>
    public interface ISolverFactory<out TSolver> where TSolver : class, ISolver
    {
        TSolver CreateSolver(SolverAccount account);
    }
}
