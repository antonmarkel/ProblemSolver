using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.Results;
using ProblemSolver.UI.DL.Auth;

namespace ProblemSolver.Logic.DlServices.Interfaces
{
    public interface IAuthService
    {
        Task<OneOf<Success, WrongCredentials>> LoginAsync(LoginRequest request, HttpClient client);
    }
}
