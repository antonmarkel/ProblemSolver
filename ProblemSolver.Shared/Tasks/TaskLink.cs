namespace ProblemSolver.Shared.Tasks
{
    public class TaskLink
    {
        public required string Url { get; set; }
        public required long TaskId { get; set; }
        public required long CourseId { get; set; }
    }
}
