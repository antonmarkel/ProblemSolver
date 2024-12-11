using ProblemSolver.Shared.Bot.Dtos.Requests;

namespace ProblemSolver.Logic.BotServices.Interfaces
{
    public interface ITaskRequestConverter
    {
        public string ConvertToMessage(TaskRequest request);
    }
}
