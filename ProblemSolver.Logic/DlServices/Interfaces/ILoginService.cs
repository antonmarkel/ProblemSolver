using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.Results;
using ProblemSolver.UI.DL.Auth;

namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ILoginService
{
    Task<OneOf<Success, WrongCredentials>> LoginAsync(LoginInfo request, HttpClient client);
}