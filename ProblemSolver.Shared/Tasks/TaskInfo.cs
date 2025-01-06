namespace ProblemSolver.Shared.Tasks;

public class TaskInfo
{
    public required long CourseId { get; set; }
    public required long TaskId { get; set; }
    public string? Task { get; set; }
    public bool Extracted { get; set; } = true;
}