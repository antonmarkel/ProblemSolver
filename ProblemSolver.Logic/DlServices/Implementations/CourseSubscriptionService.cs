using OneOf;
using OneOf.Types;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Results;

namespace ProblemSolver.Logic.DlServices.Implementations;

public class CourseSubscriptionService : ICourseSubscriptionService
{
    public async Task<OneOf<Success, Failed>> EnsureSubscriptionToCourseAsync(long courseId, HttpClient client)
    {
        string baseUrl = "selcourses1.asp";

        string fullUrl = $"{baseUrl}?yes={courseId}";
        var result = await client.GetAsync(fullUrl);
        if (result.IsSuccessStatusCode)
        {
            return new Success();
        }
        return new Failed();
    }
}