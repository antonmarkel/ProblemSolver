namespace ProblemSolver.Shared.Tasks.Enums
{
    public enum TaskState : byte
    {
        None = 0,
        Awaiting = 1,
        Solving = 2,
        Checking = 3,
        Solved = 4,
        NotSolved = 5
    }
}
