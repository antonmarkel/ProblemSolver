using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Logic.SolverServices.Interfaces
{
    public interface ISolverFactory<out TSolver> where TSolver : class, ISolver
    {
        TSolver CreateSolver(SolverAccount account);
    }
}
