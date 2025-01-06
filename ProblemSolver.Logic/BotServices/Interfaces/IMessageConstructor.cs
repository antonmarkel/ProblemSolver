using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Logic.BotServices.Interfaces
{
    public interface IMessageConstructor
    {
        public string ConvertToMessage(string task, ProgrammingLanguageEnum language, string[]? additionalProps = null);
    }
}
