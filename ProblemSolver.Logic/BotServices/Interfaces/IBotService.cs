using OneOf;
using ProblemSolver.Logic.Results;
using ProblemSolver.Shared.Bot.Dtos.Requests;

namespace ProblemSolver.Logic.BotServices.Interfaces
{
    public interface IBotService
    {
        Task<OneOf<string, Failed>> ProcessRequestAsync(SolutionRequest request);
    }
}
