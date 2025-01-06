using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Logic.BotServices.Interfaces
{
    /// <summary>
    ///     Interface responsible for constructing a message that then will be sent to AI
    /// </summary>
    public interface IMessageConstructor
    {
        public string ConvertToMessage(string task, ProgrammingLanguageEnum language, string[]? additionalProps = null);
    }
}
