namespace ProblemSolver.Shared.Tasks;

public class TaskInfo
{
    public required long TaskId { get; set; }
    public string? Info { get; set; }
    public required bool IsExtracted { get; set; }
}