using OneOf;
using ProblemSolver.Logic.DlServices.Interfaces;
using ProblemSolver.Logic.Helpers;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Tasks;

namespace ProblemSolver.Logic.DlServices.Implementations;

public class TaskExtractor : ITaskExtractor
{
    private readonly ITaskLinkExtractor _linkExtractor;

    public TaskExtractor(ITaskLinkExtractor linkExtractor)
    {
        _linkExtractor = linkExtractor;
    }

    public async Task<OneOf<List<TaskInfo>, Failed>> ExtractTasksAsync(long courseId, HttpClient client)
    {
        var linksResult = await _linkExtractor.ExtractFromCourseAsync(courseId, client);

        if (linksResult.IsT1)
            return linksResult.AsT1;

        var result = new List<TaskInfo>(linksResult.AsT0.Count);

        foreach (var link in linksResult.AsT0)
            result.Add(await ExtractTaskAsync(link, client));

        return result;
    }

    public async Task<TaskInfo> ExtractTaskAsync(TaskLink link, HttpClient client)
    {
        var info = await TextExtractor.ProcessTaskAsync(link, client);

        return info;
    }
}