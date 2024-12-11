using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Bot.Dtos.Responses;

namespace ProblemSolver.Logic.BotServices.Interfaces
{
    public interface IBotService
    {
        Task<TaskResponse> ProcessRequestAsync(TaskRequest request);
    }
}
