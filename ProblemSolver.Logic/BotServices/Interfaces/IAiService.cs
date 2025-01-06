using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Bot.Dtos.Requests;

namespace ProblemSolver.Logic.BotServices.Interfaces
{
    /// <summary>
    ///     Interface that responsible for communicating with ai.
    /// </summary>
    public interface IAiService
    {
        //TODO: It shouldn't be dependent on solution request, just abstract message. Cause in the future maybe ai won't just solve tasks. It will extract info of task from web-site
        Task<OneOf<string, Failed>> ProcessRequestAsync(SolutionRequest request);
    }
}
