using System.Text;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Logic.BotServices.Implementations.TaskRequestConverters;

public class StandardMessageConstructor : IMessageConstructor
{
    public string ConvertToMessage(string task, ProgrammingLanguageEnum language, string[]? additionalProps = null)
    {
        var messageBuilder = new StringBuilder();
        foreach (string prop in additionalProps ?? [])
            messageBuilder.AppendLine(prop);

        messageBuilder.AppendLine(
            $"You have to write a solution to the task You must follow the input/output examples.Here's raw text of task info: {task}");
        messageBuilder.AppendLine($"Solve using: {language}");
        messageBuilder.AppendLine("Send only code. Without anything at all!");

        return messageBuilder.ToString();
    }
}