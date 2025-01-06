using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.DL.Models;

namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ILoginService
{
    Task<OneOf<Success, WrongCredentials>> LoginAsync(SolverAccount account, HttpClient client);
}