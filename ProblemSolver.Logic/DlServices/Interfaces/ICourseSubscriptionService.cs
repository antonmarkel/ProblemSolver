namespace ProblemSolver.Logic.DlServices.Interfaces;

public interface ICourseSubscriptionService
{
    Task EnsureSubscriptionToCourseAsync(long courseId, HttpClient client);
}