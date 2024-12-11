using System.Text;
using ProblemSolver.Logic.BotServices.Interfaces;
using ProblemSolver.Shared.Bot.Dtos.Requests;
using ProblemSolver.Shared.Tasks;
using ProblemSolver.Shared.Tasks.Examples;

namespace ProblemSolver.Logic.BotServices.Implementations.TaskRequestConverters
{
    public class StandardConverter : ITaskRequestConverter
    {
        public string ConvertToMessage(TaskRequest request)
        {
            var messageBuilder = new StringBuilder();

            messageBuilder.AppendLine($"Task: {request.Task.TaskText ?? "Check the examples"}");
            messageBuilder.AppendLine($"Solve using {request.Language}");
            if (request.Task.Config != null)
                AddConfig(messageBuilder, request.Task.Config);

            if (request.Task.FormatExamples != null)
                AddFormatExample(messageBuilder, request.Task.FormatExamples);

            if (request.Task.SolutionExamples != null)
                AddSolutionExample(messageBuilder, request.Task.SolutionExamples);

            return messageBuilder.ToString();
        }

        private void AddConfig(StringBuilder messageBuilder, TaskConfig config)
        {
            if (config.TimeLimit != null)
                messageBuilder.AppendLine($"Time limit: {config.TimeLimit} seconds");

            if (config.MemoryLimit != null)
                messageBuilder.AppendLine($"Memory limit: {config.MemoryLimit} megabytes");

            messageBuilder.AppendLine($"Input should go from {config.InputFile ?? "console"}");
            messageBuilder.AppendLine($"Output should go to {config.OutputFile ?? "console"}");
        }

        private void AddFormatExample(StringBuilder messageBuilder, List<FormatExample> examples)
        {
            for (int i = 1; i <= examples.Count; i++)
                messageBuilder.AppendLine(
                    $"Format example{i}: input:{examples[i - 1].Input}\noutput:{examples[i - 1].Output}");
        }

        private void AddSolutionExample(StringBuilder messageBuilder, List<SolutionExample> examples)
        {
            for (int i = 1; i <= examples.Count; i++)
                messageBuilder.AppendLine(
                    $"Solution example{i}: input:{examples[i - 1].InputExample}\noutput:{examples[i - 1].OutputExample}");
        }
    }
}
