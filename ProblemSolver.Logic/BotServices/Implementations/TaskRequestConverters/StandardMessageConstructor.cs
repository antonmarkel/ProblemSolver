using System.Text;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Enums;

namespace ProblemSolver.Logic.BotServices.Implementations.TaskRequestConverters;

public class StandardMessageConstructor : IMessageConstructor
{
    public string ConvertToMessage(string task, ProgrammingLanguageEnum language)
    {
        var messageBuilder = new StringBuilder();

        messageBuilder.AppendLine(
            $"You have to write a solution to the task You must follow the input/output examples.This is the raw text of task: {task}");
        messageBuilder.AppendLine($"Solve using: {language}");
        messageBuilder.AppendLine("Send only code.");
        
        return messageBuilder.ToString();
    }
}