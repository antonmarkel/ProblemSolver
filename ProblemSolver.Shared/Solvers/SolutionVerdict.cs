namespace ProblemSolver.Shared.Solvers
{
    public class SolutionVerdict
    {
        public long TaskId { get; set; }
        public long CourseId { get; set; }
        public bool IsSolved { get; set; }
        public string? Verdict { get; set; }
    }
}
