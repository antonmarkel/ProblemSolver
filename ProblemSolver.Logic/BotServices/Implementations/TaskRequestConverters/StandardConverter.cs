using System.Text;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;

namespace ProblemSolver.Logic.BotServices.Implementations.TaskRequestConverters;

public class StandardConverter : ITaskRequestConverter
{
    public string ConvertToMessage(TaskRequest request)
    {
        var messageBuilder = new StringBuilder();

        messageBuilder.AppendLine(
            $"Here will be task web page. You must extract task from there, there could be some input/output configuration you'll have to follow: {request.Task.Info}");
        messageBuilder.AppendLine($"Solve using: {request.Language}");
        //messageBuilder.AppendLine(
        //   "Делай вывод по примеру,ничего лишнего выводить не надо. Если сказано сделать вывод в файл,так и сделай.");

        return messageBuilder.ToString();
    }
}