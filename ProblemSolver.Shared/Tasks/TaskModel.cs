using ProblemSolver.Shared.Bot.Enums;
using ProblemSolver.Shared.Tasks.Examples;

namespace ProblemSolver.Shared.Tasks;

public class TaskModel
{
    public string? TaskText { get; set; }
    public required ProgrammingLanguageEnum Language { get; set; }
    public TaskConfig? Config { get; set; }
    public List<SolutionExample>? SolutionExamples { get; set; }
    public List<FormatExample>? FormatExamples { get; set; }
}