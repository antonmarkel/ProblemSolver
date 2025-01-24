using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.Results;

namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ICourseSubscriptionService
{
    Task<OneOf<Success, Failed>> EnsureSubscriptionToCourseAsync(long courseId, HttpClient client);
}