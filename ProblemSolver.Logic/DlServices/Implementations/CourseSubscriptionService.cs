using ProblemSolver.Logic.DlServices.Interfaces;

namespace ProblemSolver.Logic.DlServices.Implementations;

public class CourseSubscriptionService : ICourseSubscriptionService
{
    public async Task EnsureSubscriptionToCourseAsync(long courseId, HttpClient client)
    {
        string baseUrl = "selcourses1.asp";

        string fullUrl = $"{baseUrl}?yes={courseId}";
        _ = await client.GetAsync(fullUrl);
    }
}