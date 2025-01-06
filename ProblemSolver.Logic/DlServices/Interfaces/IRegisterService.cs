using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.DL.Auth;
using ProblemSolver.Shared.Solvers;

namespace ProblemSolver.Logic.DlServices.Interfaces
{
    public interface IRegisterService
    {
        Task<OneOf<RegisterInfo, Failed>> RegisterAsync(SolverSettings solverSettings, HttpClient client);
    }
}
