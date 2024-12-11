using ProblemSolver.Shared.Tasks.Examples;

namespace ProblemSolver.Shared.Tasks;

public class TaskModel
{
    public string? TaskText { get; set; }
    public TaskConfig? Config { get; set; }
    public List<SolutionExample>? SolutionExamples { get; set; }
    public List<FormatExample>? FormatExamples { get; set; }
}